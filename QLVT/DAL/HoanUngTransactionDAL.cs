using Microsoft.Data.SqlClient;
using QLVT.BLL;
using QLVT.ERP.DAL;
using QLVT.ERP.Models;
using QLVT.Models;
using QLVT.Utils;
using System.Data;

namespace QLVT.DAL
{
    public class HoanUngTransactionDAL
    {
        private readonly WarehouseDAL _warehouseDAL;
        public HoanUngTransactionDAL()
        {
            _warehouseDAL = new WarehouseDAL();
        }

        #region Mạng cấp 4
        public List<DonDangKyModel> MC4_GetDanhSachDonDangKy()
        {
            var result = new List<DonDangKyModel>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT Id, MADDK, TENKH, DiaChi, NhanVienKyThuat, 
                           MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, 
                           NgayHoanUng, DaHoanUng, ThoiGianXacNhanHoanUng, 
                           MaNVXacNhan, CreatedDate, UpdatedDate
                    FROM ct.DonDangKy 
                    WHERE DaHoanUng is null OR DaHoanUng = 0
                    ORDER BY NgayHoanUng";

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new DonDangKyModel
                            {
                                Id = reader.GetInt32("Id"),
                                MADDK = reader.GetString("MADDK"),
                                TENKH = reader.GetString("TENKH"),
                                DiaChi = reader.GetString("DiaChi"),
                                NhanVienKyThuat = reader.GetString("NhanVienKyThuat"),
                                MaNhanVienXayLap = reader.GetString("MaNhanVienXayLap"),
                                NhanVienXayLap = reader.GetString("NhanVienXayLap"),
                                NgayHoanThanh = reader.IsDBNull("NgayHoanThanh") ? null : reader.GetDateTime("NgayHoanThanh"),
                                NgayHoanUng = reader.IsDBNull("NgayHoanUng") ? null : reader.GetDateTime("NgayHoanUng"),
                            });
                        }
                    }
                }
            }

            return result;
        }
        public bool MC4_UpdateHoanUngDonDangKy(string maddk, string nguoiXacNhan)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Bước 0: Lấy thông tin đơn đăng ký để kiểm tra và lấy mã nhân viên xây lắp
                        var donDangKy = MC4_GetDonDangKyByMa(maddk, connection, transaction);
                        if (donDangKy == null)
                        {
                            throw new Exception("Không tìm thấy hồ sơ");
                        }

                        // Kiểm tra hồ sơ đã hoàn ứng chưa
                        if (donDangKy.DaHoanUng == true)
                        {
                            throw new Exception("Hồ sơ đã được hoàn ứng trước đó");
                        }

                        // Bước 0.1: Tìm kho dựa trên mã nhân viên xây lắp
                        var wh = _warehouseDAL.GetKhoUuTienByMaNV(donDangKy.MaNhanVienXayLap);

                        if (wh == null)
                        {
                            throw new Exception($"Không tìm thấy kho cho nhân viên: {donDangKy.MaNhanVienXayLap}");
                        }

                        // Bước 1: Cập nhật trạng thái hoàn ứng trong DonDangKy
                        string updateDonSql = @"
                            UPDATE ct.DonDangKy 
                            SET DaHoanUng = 1,
                                ThoiGianXacNhanHoanUng = @thoiGianXacNhan,
                                MaNVXacNhan = @nguoiXacNhan,
                                UpdatedDate = @updateDate
                            WHERE MADDK = @maDDK AND (DaHoanUng IS NULL OR DaHoanUng = 0)";

                        using (var command = new SqlCommand(updateDonSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@maDDK", maddk);
                            command.Parameters.AddWithValue("@nguoiXacNhan", nguoiXacNhan);
                            command.Parameters.AddWithValue("@thoiGianXacNhan", DateTime.Now);
                            command.Parameters.AddWithValue("@updateDate", DateTime.Now);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        // Bước 2: Tạo transaction hoàn ứng
                        string soPhieu = GenerateHoanUngTransactionNumber();
                        string insertTransactionSql = @"
                            INSERT INTO Transactions 
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaNV, MaKhoNguon, GhiChu, CreatedBy, EntityHoanUng)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'HoanUng', @maNV, @maKhoNguon, @ghiChu, @createdBy, @entityHoanUng);
                            SELECT SCOPE_IDENTITY();";

                        int transactionId;
                        using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@soPhieu", soPhieu);
                            command.Parameters.AddWithValue("@ngayGiaoDich", donDangKy.NgayHoanUng);
                            command.Parameters.AddWithValue("@maNV", donDangKy.MaNhanVienXayLap);
                            command.Parameters.AddWithValue("@maKhoNguon", wh.Id); // Kho của nhân viên xây lắp
                            command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng vật tư cho đơn đăng ký: {maddk}");
                            command.Parameters.AddWithValue("@createdBy", nguoiXacNhan);
                            command.Parameters.AddWithValue("@entityHoanUng", maddk);

                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Bước 3: Tạo transaction chi tiết và cập nhật tồn kho
                        var chiTietList = MC4_GetChiTietVatTuByMaDDK(maddk, connection, transaction);
                        foreach (var chiTiet in chiTietList)
                        {
                            if (chiTiet.SoLuongHoanUng > 0)
                            {
                                // Insert transaction detail
                                string insertDetailSql = @"
                                    INSERT INTO TransactionDetails (TransactionId, ErpId, SoLuong, GhiChu, MaKhoXuat, CreatedBy)
                                    VALUES (@transactionId, @erpId, @soLuong, @ghiChu, @maKhoXuat, @createdBy)";

                                using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@transactionId", transactionId);
                                    command.Parameters.AddWithValue("@erpId", chiTiet.MaVTErp);
                                    command.Parameters.AddWithValue("@soLuong", chiTiet.SoLuongHoanUng);
                                    command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng vật tư MC4: {maddk}");
                                    command.Parameters.AddWithValue("@maKhoXuat", wh.Id);
                                    command.Parameters.AddWithValue("@createdBy", nguoiXacNhan);

                                    command.ExecuteNonQuery();
                                }

                                // Cập nhật tồn kho từ kho của nhân viên xây lắp
                                CapNhatTonKhoHoanUng(connection, transaction, wh.Id, chiTiet.MaVTErp, chiTiet.SoLuongHoanUng);
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw; // Ném lại exception để BLL và GUI có thể hiển thị chi tiết lỗi
                    }
                }
            }
        }
        private DonDangKyModel? MC4_GetDonDangKyByMa(string maddk, SqlConnection connection, SqlTransaction transaction)
        {
            string sql = @"
                SELECT Id, MADDK, TENKH, DiaChi, NhanVienKyThuat, 
                       MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, 
                       NgayHoanUng, DaHoanUng, ThoiGianXacNhanHoanUng, 
                       MaNVXacNhan, CreatedDate, UpdatedDate
                FROM ct.DonDangKy 
                WHERE MADDK = @maDDK";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@maDDK", maddk);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DonDangKyModel
                        {
                            Id = reader.GetInt32("Id"),
                            MADDK = reader.GetString("MADDK"),
                            TENKH = reader.GetString("TENKH"),
                            DiaChi = reader.GetString("DiaChi"),
                            NhanVienKyThuat = reader.GetString("NhanVienKyThuat"),
                            MaNhanVienXayLap = reader.GetString("MaNhanVienXayLap"),
                            NhanVienXayLap = reader.GetString("NhanVienXayLap"),
                            NgayHoanThanh = reader.IsDBNull("NgayHoanThanh") ? null : reader.GetDateTime("NgayHoanThanh"),
                            NgayHoanUng = reader.IsDBNull("NgayHoanUng") ? null : reader.GetDateTime("NgayHoanUng"),
                            DaHoanUng = reader.IsDBNull("DaHoanUng") ? null : reader.GetBoolean("DaHoanUng"),
                            ThoiGianXacNhanHoanUng = reader.IsDBNull("ThoiGianXacNhanHoanUng") ? null : reader.GetDateTime("ThoiGianXacNhanHoanUng"),
                            MaNVXacNhan = reader.IsDBNull("MaNVXacNhan") ? "" : reader.GetString("MaNVXacNhan"),
                            CreatedDate = reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.GetDateTime("UpdatedDate")
                        };
                    }
                }
            }

            return null;
        }
        private List<DonDangKyCTModel> MC4_GetChiTietVatTuByMaDDK(string maddk, SqlConnection connection, SqlTransaction transaction)
        {
            var result = new List<DonDangKyCTModel>();

            string sql = @"
                SELECT Id, MADDK, MaVTErp, SoLuongHoanUng, CreatedDate
                FROM ct.DonDangKyCT 
                WHERE MADDK = @maDDK";
             
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@maDDK", maddk);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DonDangKyCTModel
                        {
                            Id = reader.GetInt32("Id"),
                            MADDK = reader.GetString("MADDK"),
                            MaVTErp = reader.GetInt32("MaVTErp"),
                            SoLuongHoanUng = reader.GetDecimal("SoLuongHoanUng"),
                            CreatedDate = reader.GetDateTime("CreatedDate")
                        });
                    }
                }
            }

            return result;
        }
        public DateTime? MC4_GetMaxNgayHoanUngMC4()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT MAX(NgayHoanUng) FROM ct.DonDangKy";

                using (var command = new SqlCommand(sql, connection))
                {
                    var result = command.ExecuteScalar();
                    return result == DBNull.Value ? null : (DateTime?)result;
                }
            }
        }
        public List<DonDangKyCTModel> MC4_GetChiTietVatTuWithTonKho(string maddk)
        {
            var result = new List<DonDangKyCTModel>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT ct.Id, ct.MADDK, ct.MaVTErp, ct.SoLuongHoanUng, vt.TenVatTu, vt.DVT, vt.Code as MaVT,
                            ISNULL(tk.TonKho, 0) as TonKho
                    FROM ct.DonDangKy ddk
                        INNER JOIN ct.DonDangKyCT ct ON ddk.MADDK = ct.MADDK
                        INNER JOIN ViewVatTus vt ON vt.ErpId = ct.MaVTErp
                        LEFT JOIN Warehouses w ON w.MaNV = ddk.MaNhanVienXayLap
                        LEFT JOIN ViewTonKhoVatTu tk ON tk.WarehouseId = w.Id and tk.SupplyErpId = ct.MaVTErp
                    WHERE ddk.MADDK  = @maddk";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@maddk", maddk);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chiTiet = new DonDangKyCTModel
                            {
                                Id = reader.GetInt32("Id"),
                                MADDK = reader["MADDK"].ToString() ?? string.Empty,
                                MaVTErp = reader.GetInt32("MaVTErp"),
                                TenVT = reader["TenVatTu"].ToString() ?? string.Empty,
                                MaVT = reader["MaVT"].ToString() ?? string.Empty,
                                DVT = reader["DVT"].ToString() ?? string.Empty,
                                SoLuongHoanUng = reader["SoLuongHoanUng"] != DBNull.Value ? Convert.ToDecimal(reader["SoLuongHoanUng"]) : 0,
                                TonKho = reader["TonKho"] != DBNull.Value ? Convert.ToDecimal(reader["TonKho"]) : 0
                            };

                            result.Add(chiTiet);
                        }
                    }
                }
            }

            return result;
        }
        public bool MC4_IsDonDangKyExists(string maDDK)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM ct.DonDangKy WHERE MADDK = @maddk";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@maddk", maDDK);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public bool MC4_InsertDonDangKyWithChiTiet(DonDangKyModel don, List<DonDangKyCTModel> chiTietList)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Bước 1: Thêm đơn đăng ký (chỉ các cột tồn tại trong database)
                        string sqlDon = @"
                            INSERT INTO ct.DonDangKy (MADDK, TENKH, DiaChi, NhanVienKyThuat, 
                                                    MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, 
                                                    NgayHoanUng, DaHoanUng)
                            VALUES (@maddk, @tenkh, @diachi, @nvkt, @manvxl, @nvxl, @ngayht, @ngayhu, @dahoanung)";

                        using (var commandDon = new SqlCommand(sqlDon, connection, transaction))
                        {
                            commandDon.Parameters.AddWithValue("@maddk", don.MADDK);
                            commandDon.Parameters.AddWithValue("@tenkh", don.TENKH ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@diachi", don.DiaChi ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@nvkt", don.NhanVienKyThuat ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@manvxl", don.MaNhanVienXayLap ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@nvxl", don.NhanVienXayLap ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@ngayht", don.NgayHoanThanh ?? (object)DBNull.Value);
                            commandDon.Parameters.AddWithValue("@ngayhu", don.NgayHoanUng ?? (object)DBNull.Value);
                            commandDon.Parameters.AddWithValue("@dahoanung", don.DaHoanUng ?? (object)DBNull.Value);

                            int donResult = commandDon.ExecuteNonQuery();
                            if (donResult <= 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        // Bước 2: Thêm chi tiết vật tư (chỉ các cột tồn tại trong database)
                        if (chiTietList != null && chiTietList.Count > 0)
                        {
                            string sqlChiTiet = @"
                                INSERT INTO ct.DonDangKyCT (MADDK, MaVTErp, SoLuongHoanUng)
                                VALUES (@maddk, @maVTErp, @soLuongHoanUng)";

                            foreach (var chiTiet in chiTietList)
                            {
                                using (var commandChiTiet = new SqlCommand(sqlChiTiet, connection, transaction))
                                {
                                    commandChiTiet.Parameters.AddWithValue("@maddk", chiTiet.MADDK);
                                    commandChiTiet.Parameters.AddWithValue("@maVTErp", chiTiet.MaVTErp);
                                    commandChiTiet.Parameters.AddWithValue("@soLuongHoanUng", chiTiet.SoLuongHoanUng);

                                    int chiTietResult = commandChiTiet.ExecuteNonQuery();
                                    if (chiTietResult <= 0)
                                    {
                                        transaction.Rollback();
                                        return false;
                                    }
                                }
                            }
                        }

                        // Commit transaction nếu tất cả thành công
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        #endregion

        #region Điểm chảy
        public List<SuaChuaModel> DC_GetDanhSachDonSuaChua()
        {
            var result = new List<SuaChuaModel>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT Id, MADON, ViTriDiemChay, 
                            MaNhanVienXayLap, NhanVienXayLap,
                            NgayHoanThanh, NgayHoanUng, DaHoanUng, ThoiGianXacNhanHoanUng, 
                            MaNVXacNhan, CreatedDate, UpdatedDate
                    FROM ct.SuaChua
                    WHERE DaHoanUng is null OR DaHoanUng = 0
                    ORDER BY NgayHoanUng DESC";

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new SuaChuaModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                MADON = reader["MADON"].ToString() ?? string.Empty,
                                ViTriDiemChay = reader["ViTriDiemChay"].ToString() ?? string.Empty,
                                MaNhanVienXayLap = reader["MaNhanVienXayLap"].ToString() ?? string.Empty,
                                NhanVienXayLap = reader["NhanVienXayLap"].ToString() ?? string.Empty,
                                NgayHoanThanh = reader["NgayHoanThanh"] != DBNull.Value ? Convert.ToDateTime(reader["NgayHoanThanh"]) : null,
                                NgayHoanUng = reader["NgayHoanUng"] != DBNull.Value ? Convert.ToDateTime(reader["NgayHoanUng"]) : null
                            });
                        }
                    }
                }
            }

            return result;
        }
        public bool DC_UpdateHoanUngSuaChua(string maDon, string nguoiXacNhan)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Bước 0: Lấy thông tin đơn đăng ký để kiểm tra và lấy mã nhân viên xây lắp
                            var suaChua = DC_GetDonSuaChuaByMa(maDon, connection, transaction);
                            if (suaChua == null)
                            {
                                throw new Exception("Không tìm thấy hồ sơ");
                            }

                            // Kiểm tra hồ sơ đã hoàn ứng chưa
                            if (suaChua.DaHoanUng == true)
                            {
                                throw new Exception("Hồ sơ đã được hoàn ứng trước đó");
                            }

                            // Bước 0.1: Tìm kho dựa trên mã nhân viên xây lắp
                            var wh = _warehouseDAL.GetKhoUuTienByMaNV(suaChua.MaNhanVienXayLap); 
                            if (wh == null)
                            {
                                throw new Exception($"Không tìm thấy kho cho nhân viên: {suaChua.MaNhanVienXayLap}");
                            }

                            // Bước 1: Cập nhật trạng thái hoàn ứng trong bảng ct.SuaChua
                            string sqlUpdateSuaChua = @"
                                UPDATE ct.SuaChua 
                                SET DaHoanUng = 1,
                                    MaNVXacNhan = @nguoiXacNhan,
                                    ThoiGianXacNhanHoanUng = @thoiGianXacNhan,
                                    UpdatedDate = @updateDate
                                WHERE MADON = @maDon AND (DaHoanUng IS NULL OR DaHoanUng = 0)";

                            using (var command = new SqlCommand(sqlUpdateSuaChua, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@maDon", maDon);
                                command.Parameters.AddWithValue("@nguoiXacNhan", nguoiXacNhan);
                                command.Parameters.AddWithValue("@thoiGianXacNhan", DateTime.Now);
                                command.Parameters.AddWithValue("@updateDate", DateTime.Now);


                                int rows = command.ExecuteNonQuery();
                                if (rows == 0)
                                {
                                    throw new Exception("Không tìm thấy đơn sửa chữa để cập nhật");
                                }
                            }

                            // Bước 2: Lấy chi tiết vật tư để tạo transaction
                            var chiTietList = DC_GetChiTietSuaChuaByMaDon(maDon, connection, transaction);
                            if (chiTietList.Count == 0)
                            {
                                throw new Exception("Không tìm thấy chi tiết vật tư cho đơn sửa chữa");
                            }

                            // Bước 3: Tạo transaction hoàn ứng
                            int transactionId = CreateHoanUngTransaction(maDon, "SuaChua", nguoiXacNhan, suaChua.NgayHoanUng.GetValueOrDefault(), suaChua.MaNhanVienXayLap, wh.Id, connection, transaction);
                            // Bước 4: Tạo chi tiết transaction và cập nhật tồn kho
                            foreach (var chiTiet in chiTietList)
                            {
                                string insertDetailSql = @"
                                    INSERT INTO TransactionDetails (TransactionId, ErpId, SoLuong, GhiChu, MaKhoXuat, CreatedBy)
                                    VALUES (@transactionId, @erpId, @soLuong, @ghiChu, @maKhoXuat, @createdBy)";

                                using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@transactionId", transactionId);
                                    command.Parameters.AddWithValue("@erpId", chiTiet.MaVTErp);
                                    command.Parameters.AddWithValue("@soLuong", chiTiet.SoLuongHoanUng);
                                    command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng vật tư ĐC: {maDon}");
                                    command.Parameters.AddWithValue("@maKhoXuat", wh.Id);
                                    command.Parameters.AddWithValue("@createdBy", nguoiXacNhan);

                                    command.ExecuteNonQuery();
                                }

                                // Cập nhật tồn kho
                                CapNhatTonKhoHoanUng(connection, transaction, wh.Id, chiTiet.MaVTErp, chiTiet.SoLuongHoanUng);
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Lỗi thực hiện hoàn ứng sửa chữa: {ex.Message}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi kết nối database khi hoàn ứng sửa chữa: {ex.Message}", ex);
            }
        }
        private SuaChuaModel? DC_GetDonSuaChuaByMa(string maDon, SqlConnection connection, SqlTransaction transaction)
        {

            string sql = @"
                SELECT Id, MADON, ViTriDiemChay, MaNhanVienXayLap, 
                        NhanVienXayLap, NgayHoanThanh, NgayHoanUng, 
                        DaHoanUng, ThoiGianXacNhanHoanUng, MaNVXacNhan, CreatedDate, UpdatedDate
                FROM ct.SuaChua 
                WHERE MADON = @madon";

            using (var command = new SqlCommand(sql, connection, transaction))
            {

                command.Parameters.AddWithValue("@madon", maDon);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SuaChuaModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            MADON = reader["MADON"].ToString() ?? string.Empty,
                            ViTriDiemChay = reader["ViTriDiemChay"].ToString() ?? string.Empty,
                            MaNhanVienXayLap = reader["MaNhanVienXayLap"].ToString() ?? string.Empty,
                            NhanVienXayLap = reader["NhanVienXayLap"].ToString() ?? string.Empty,
                            NgayHoanThanh = reader["NgayHoanThanh"] != DBNull.Value ? Convert.ToDateTime(reader["NgayHoanThanh"]) : null,
                            NgayHoanUng = reader["NgayHoanUng"] != DBNull.Value ? Convert.ToDateTime(reader["NgayHoanUng"]) : null,
                            DaHoanUng = reader["DaHoanUng"] != DBNull.Value ? Convert.ToBoolean(reader["DaHoanUng"]) : null,
                            ThoiGianXacNhanHoanUng = reader["ThoiGianXacNhanHoanUng"] != DBNull.Value ? Convert.ToDateTime(reader["ThoiGianXacNhanHoanUng"]) : null,
                            MaNVXacNhan = reader["MaNVXacNhan"].ToString() ?? string.Empty,
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                            UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"])
                        };
                    }
                }
            }

            return null;
        }
        private List<SuaChuaCTModel> DC_GetChiTietSuaChuaByMaDon(string maDon, SqlConnection connection, SqlTransaction transaction)
        {
            var result = new List<SuaChuaCTModel>();

            string sql = @"
                SELECT s.ErpId, s.TenVatTu, sc.SoLuongHoanUng
                FROM ct.SuaChuaCT sc
                    INNER JOIN Supplies s ON sc.MaVTErp = s.ErpId
                WHERE sc.MADON = @maDon";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@maDon", maDon);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new SuaChuaCTModel
                        {
                            MaVTErp = reader.GetInt32("ErpId"),
                            TenVT = reader.GetString("TenVatTu"),
                            SoLuongHoanUng = reader.GetDecimal("SoLuongHoanUng")
                        });
                    }
                }
            }

            return result;
        }
        public DateTime? DC_GetMaxNgayHoanUng()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT MAX(NgayHoanUng) FROM ct.SuaChua WHERE NgayHoanUng IS NOT NULL";

                using (var command = new SqlCommand(sql, connection))
                {
                    var result = command.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDateTime(result) : null;
                }
            }
        }
        public List<SuaChuaCTModel> DC_GetChiTietVatTuWithTonKho(string maDon)
        {
            var result = new List<SuaChuaCTModel>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT ct.Id, ct.MADON, ct.MaVTErp, ct.SoLuongHoanUng, vt.TenVatTu, vt.DVT, vt.Code as MaVT,
                            ISNULL(tk.TonKho, 0) as TonKho
                    FROM ct.SuaChua sc
                        INNER JOIN ct.SuaChuaCT ct ON sc.MADON = ct.MADON
                        INNER JOIN ViewVatTus vt ON vt.ErpId = ct.MaVTErp
                        LEFT JOIN Warehouses w ON w.MaNV = sc.MaNhanVienXayLap AND w.KhoUuTien = 1
                        LEFT JOIN ViewTonKhoVatTu tk ON tk.WarehouseId = w.Id and tk.SupplyErpId = ct.MaVTErp
                    WHERE ct.MADON = @madon";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@madon", maDon);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chiTiet = new SuaChuaCTModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                MADON = reader["MADON"].ToString() ?? string.Empty,
                                MaVTErp = reader.GetInt32("MaVTErp"),
                                TenVT = reader["TenVatTu"].ToString() ?? string.Empty,
                                MaVT = reader["MaVT"].ToString() ?? string.Empty,
                                DVT = reader["DVT"].ToString() ?? string.Empty,
                                SoLuongHoanUng = reader["SoLuongHoanUng"] != DBNull.Value ? Convert.ToDecimal(reader["SoLuongHoanUng"]) : 0,
                                TonKho = reader["TonKho"] != DBNull.Value ? Convert.ToDecimal(reader["TonKho"]) : 0
                            };

                            result.Add(chiTiet);
                        }
                    }
                }
            }

            return result;
        }
        public bool DC_IsDonSuaChuaExists(string maDon)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM ct.SuaChua WHERE MADON = @madon";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@madon", maDon);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public bool DC_InsertDonSuaChuaWithChiTiet(SuaChuaModel don, List<SuaChuaCTModel> chiTietList)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Bước 1: Thêm đơn sửa chữa
                        string sqlDon = @"
                            INSERT INTO ct.SuaChua (MADON, ViTriDiemChay, MaNhanVienXayLap, NhanVienXayLap, 
                                                    NgayHoanThanh, NgayHoanUng, CreatedDate)
                            VALUES (@madon, @vitri, @manvxl, @nvxl, 
                                    @ngayht, @ngayhu, @created)";

                        using (var commandDon = new SqlCommand(sqlDon, connection, transaction))
                        {
                            commandDon.Parameters.AddWithValue("@madon", don.MADON);
                            commandDon.Parameters.AddWithValue("@vitri", don.ViTriDiemChay ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@manvxl", don.MaNhanVienXayLap ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@nvxl", don.NhanVienXayLap ?? string.Empty);
                            commandDon.Parameters.AddWithValue("@ngayht", don.NgayHoanThanh ?? (object)DBNull.Value);
                            commandDon.Parameters.AddWithValue("@ngayhu", don.NgayHoanUng ?? (object)DBNull.Value);
                            commandDon.Parameters.AddWithValue("@created", don.CreatedDate);

                            int donResult = commandDon.ExecuteNonQuery();
                            if (donResult <= 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        // Bước 2: Thêm chi tiết vật tư
                        if (chiTietList != null && chiTietList.Count > 0)
                        {
                            string sqlChiTiet = @"
                                INSERT INTO ct.SuaChuaCT (MADON, MaVTErp, SoLuongHoanUng, CreatedDate)
                                VALUES (@madon, @mavt, @soluong, GETDATE())";

                            foreach (var chiTiet in chiTietList)
                            {
                                using (var commandChiTiet = new SqlCommand(sqlChiTiet, connection, transaction))
                                {
                                    commandChiTiet.Parameters.AddWithValue("@madon", chiTiet.MADON);
                                    commandChiTiet.Parameters.AddWithValue("@mavt", chiTiet.MaVTErp);
                                    commandChiTiet.Parameters.AddWithValue("@soluong", chiTiet.SoLuongHoanUng);

                                    int chiTietResult = commandChiTiet.ExecuteNonQuery();
                                    if (chiTietResult <= 0)
                                    {
                                        transaction.Rollback();
                                        return false;
                                    }
                                }
                            }
                        }

                        // Commit transaction nếu tất cả thành công
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        #endregion

        #region BGK (Bàn giao kỹ thuật)

        /// <summary>
        /// Xử lý hoàn ứng BGK - Lưu vào ct.NghiemThuGiaoKhoan, ct.NghiemThuGiaoKhoanCT, Transactions, TransactionDetails và cập nhật Inventory
        /// </summary>
        public bool BGK_ProcessHoanUngBGK(
            NghiemThuGiaoKhoanModel bgkModel,
            List<NghiemThuGiaoKhoanCTModel> vatTuList,
            string nguoiXacNhan)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Bước 1: Insert vào ct.NghiemThuGiaoKhoan (không check tồn tại)
                            string insertBGKSql = @"
                                INSERT INTO ct.NghiemThuGiaoKhoan 
                                (GiaoKhoanNghiemThuVatTuID, SoBGK, NhanVienKyThuat, MaNhanVienXayLap, NhanVienXayLap, 
                                 NoiDung, SoLanNghiemThu, SoNghiemThu, NamNghiemThu, NgayHoanUng, DaHoanUng, 
                                 ThoiGianXacNhanHoanUng, MaNVXacNhan, CreatedDate, UpdatedDate)
                                VALUES 
                                (@giaoKhoanID, @soBGK, @nvKyThuat, @maNVXayLap, @nvXayLap, 
                                 @noiDung, @soLanNghiemThu, @soNghiemThu, @namNghiemThu, @ngayHoanUng, 1, 
                                 @thoiGianXacNhan, @nguoiXacNhan, @createdDate, @updatedDate)";

                            using (var command = new SqlCommand(insertBGKSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@giaoKhoanID", bgkModel.GiaoKhoanNghiemThuVatTuID ?? 0);
                                command.Parameters.AddWithValue("@soBGK", bgkModel.SoBGK ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@nvKyThuat", bgkModel.NhanVienKyThuat ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@maNVXayLap", bgkModel.MaNhanVienXayLap ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@nvXayLap", bgkModel.NhanVienXayLap ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@noiDung", bgkModel.NoiDung ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@soLanNghiemThu", bgkModel.SoLanNghiemThu ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@soNghiemThu", bgkModel.SoNghiemThu ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@namNghiemThu", bgkModel.NamNghiemThu ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@ngayHoanUng", bgkModel.NgayHoanUng);
                                command.Parameters.AddWithValue("@thoiGianXacNhan", DateTime.Now);
                                command.Parameters.AddWithValue("@nguoiXacNhan", nguoiXacNhan);
                                command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                                command.Parameters.AddWithValue("@updatedDate", DateTime.Now);

                                command.ExecuteNonQuery();
                            }

                            // Bước 2: Insert chi tiết vật tư vào ct.NghiemThuGiaoKhoanCT
                            foreach (var vatTu in vatTuList)
                            {
                                string insertVatTuSql = @"
                                    INSERT INTO ct.NghiemThuGiaoKhoanCT 
                                    (GiaoKhoanNghiemThuVatTuID, MaVTErp, SoLuongHoanUng, SoLuongHoanUngThucTe, CreatedDate)
                                    VALUES 
                                    (@giaoKhoanID, @maVTErp, @soLuong, @soLuongThucTe, @createdDate)";

                                using (var command = new SqlCommand(insertVatTuSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@giaoKhoanID", bgkModel.GiaoKhoanNghiemThuVatTuID ?? 0);
                                    command.Parameters.AddWithValue("@maVTErp", vatTu.MaVTErp);
                                    command.Parameters.AddWithValue("@soLuong", vatTu.SoLuongHoanUng);
                                    // Sử dụng SoLuongHoanUngThucTe nếu có, nếu không thì dùng SoLuongHoanUng
                                    command.Parameters.AddWithValue("@soLuongThucTe", vatTu.SoLuongHoanUngThucTe ?? vatTu.SoLuongHoanUng);
                                    command.Parameters.AddWithValue("@createdDate", DateTime.Now);

                                    command.ExecuteNonQuery();
                                }
                            }

                            // Bước 3: Tìm kho dựa trên nhân viên xây lắp
                            var wh = _warehouseDAL.GetKhoUuTienByMaNV(bgkModel.MaNhanVienXayLap ?? bgkModel.NhanVienXayLap ?? "");
                            if (wh == null)
                            {
                                throw new Exception($"Không tìm thấy kho cho nhân viên: {bgkModel.NhanVienXayLap}");
                            }

                            // Bước 4: Tạo transaction hoàn ứng trong bảng Transactions
                            string soPhieu = GenerateHoanUngTransactionNumber();
                            
                            string insertTransactionSql = @"
                                INSERT INTO Transactions 
                                (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaKhoNguon, MaNV, GhiChu, CreatedBy, EntityHoanUng)
                                VALUES 
                                (@soPhieu, @ngayGiaoDich, 'HoanUng', @maKhoNguon, @maNV, @ghiChu, @createdBy, @entityHoanUng);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int transactionId;
                            using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@soPhieu", soPhieu);
                                command.Parameters.AddWithValue("@ngayGiaoDich", bgkModel.NgayHoanUng);
                                command.Parameters.AddWithValue("@maKhoNguon", wh.Id);
                                command.Parameters.AddWithValue("@maNV", bgkModel.MaNhanVienXayLap ?? bgkModel.NhanVienXayLap ?? "");
                                command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng BGK: {bgkModel.SoBGK}");
                                command.Parameters.AddWithValue("@createdBy", nguoiXacNhan);
                                command.Parameters.AddWithValue("@entityHoanUng", $"BGK:{bgkModel.SoNghiemThu}-{bgkModel.NamNghiemThu}");

                                transactionId = (int)command.ExecuteScalar();
                            }

                            // Bước 5: Tạo chi tiết giao dịch trong TransactionDetails và cập nhật Inventory
                            foreach (var vatTu in vatTuList)
                            {
                                // Số lượng thực tế để hoàn ứng (ưu tiên SoLuongHoanUngThucTe)
                                decimal soLuongThucTe = vatTu.SoLuongHoanUngThucTe ?? vatTu.SoLuongHoanUng;

                                if (soLuongThucTe == 0)
                                    break;
                                
                                // Insert vào TransactionDetails
                                string insertDetailSql = @"
                                    INSERT INTO TransactionDetails 
                                    (TransactionId, ErpId, MaKhoXuat, SoLuong, GhiChu, CreatedBy)
                                    VALUES 
                                    (@transactionId, @erpId, @maKhoXuat, @soLuong, @ghiChu, @createdBy)";

                                using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@transactionId", transactionId);
                                    command.Parameters.AddWithValue("@erpId", vatTu.MaVTErp);
                                    command.Parameters.AddWithValue("@maKhoXuat", wh.Id);
                                    command.Parameters.AddWithValue("@soLuong", soLuongThucTe);
                                    command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng BGK: {bgkModel.SoBGK}");
                                    command.Parameters.AddWithValue("@createdBy", nguoiXacNhan);

                                    command.ExecuteNonQuery();
                                }

                                // Cập nhật Inventory (trừ tồn kho vì hoàn ứng = xuất kho)
                                CapNhatTonKhoHoanUng(connection, transaction, wh.Id, vatTu.MaVTErp, soLuongThucTe);
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xử lý hoàn ứng BGK: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra BGK đã được hoàn ứng trong database chưa
        /// </summary>
        public bool CheckBGKDaHoanUng(long giaoKhoanNghiemThuVatTuID)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                        SELECT COUNT(*) 
                        FROM ct.NghiemThuGiaoKhoan 
                        WHERE GiaoKhoanNghiemThuVatTuID = @giaoKhoanID 
                        AND DaHoanUng = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@giaoKhoanID", giaoKhoanNghiemThuVatTuID);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi kiểm tra BGK đã hoàn ứng: {ex.Message}", ex);
            }
        }
        #endregion

        #region Dùng chung
        private string GenerateHoanUngTransactionNumber()
        {
            string dateStr = DateTime.Now.ToString("yyMMdd");
            string prefix = "HU"; // Hoàn Ứng

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    string sql = @"
                        SELECT COUNT(*) 
                        FROM Transactions 
                        WHERE LoaiGiaoDich = 'HoanUng' 
                        AND CONVERT(DATE, CreatedDate) = CONVERT(DATE, GETDATE())";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        return $"{prefix}{dateStr}-{(count + 1):D4}";
                    }
                }
            }
            catch
            {
                // Fallback với timestamp
                return $"{prefix}{dateStr}{DateTime.Now:HHmmss}";
            }
        }
        private int CreateHoanUngTransaction(string maDon, string loaiDon, string nguoiXacNhan,
            DateTime ngayHoanUng, string manvXayLap, int maKho, SqlConnection connection, SqlTransaction transaction)
        {
            string insertTransactionSql = @"INSERT INTO Transactions 
                            (SoPhieu, NgayGiaoDich, LoaiGiaoDich, MaNV, MaKhoNguon, GhiChu, CreatedBy, EntityHoanUng)
                            VALUES 
                            (@soPhieu, @ngayGiaoDich, 'HoanUng', @maNV, @maKhoNguon, @ghiChu, @createdBy, @entityHoanUng);
                            SELECT SCOPE_IDENTITY();";

            string soPhieu = GenerateHoanUngTransactionNumber();

            using (var command = new SqlCommand(insertTransactionSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@soPhieu", soPhieu);
                command.Parameters.AddWithValue("@ngayGiaoDich", ngayHoanUng);
                command.Parameters.AddWithValue("@maNV", manvXayLap);
                command.Parameters.AddWithValue("@maKhoNguon", maKho); // Kho của nhân viên xây lắp
                if (loaiDon == "MangCap4")
                    command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng vật tư cho đơn MC4: {maDon}");
                else
                    command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng vật tư cho đơn sửa chữa: {maDon}");
                command.Parameters.AddWithValue("@createdBy", nguoiXacNhan);
                command.Parameters.AddWithValue("@entityHoanUng", maDon);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void CapNhatTonKhoHoanUng(SqlConnection connection, SqlTransaction transaction, int warehouseId, int erpId, decimal quantity)
        {
            // Kiểm tra tồn kho hiện tại tại kho này
            string checkSql = @"
                SELECT SoLuongTon 
                FROM Inventory 
                WHERE WarehouseId = @warehouseId AND SupplyErpId = @erpId";

            using (var command = new SqlCommand(checkSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@warehouseId", warehouseId);
                command.Parameters.AddWithValue("@erpId", erpId);

                var result = command.ExecuteScalar();
                decimal currentStock = result != null ? Convert.ToDecimal(result) : 0;

                if (currentStock < quantity)
                {
                    throw new Exception($"Không đủ tồn kho để hoàn ứng. Kho {warehouseId} - VT {erpId}: Tồn {currentStock}, Cần {quantity}");
                }

                // Trừ tồn kho
                string updateSql = @"
                    UPDATE Inventory 
                    SET SoLuongTon = SoLuongTon - @quantity, LastUpdated = GETDATE()
                    WHERE WarehouseId = @warehouseId AND SupplyErpId = @erpId";

                using (var updateCommand = new SqlCommand(updateSql, connection, transaction))
                {
                    updateCommand.Parameters.AddWithValue("@warehouseId", warehouseId);
                    updateCommand.Parameters.AddWithValue("@erpId", erpId);
                    updateCommand.Parameters.AddWithValue("@quantity", quantity);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Không thể cập nhật tồn kho cho kho {warehouseId} - VT {erpId}");
                    }
                }
            }
        }
        #endregion
    }
}
