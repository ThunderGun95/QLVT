using Microsoft.Data.SqlClient;
using QLVT.ERP.Models;
using QLVT.Utils;
using System.Data;

namespace QLVT.DAL
{
    public class HoanUngTransactionDAL
    {
        #region Mạng cấp 4
        public List<DonDangKyModel> GetDanhSachDonDangKy()
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

        public DonDangKyModel? GetDonDangKyByMa(string maddk)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT Id, MADDK, TENKH, DiaChi, NhanVienKyThuat, 
                           MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, 
                           NgayHoanUng, DaHoanUng, ThoiGianXacNhanHoanUng, 
                           MaNVXacNhan, CreatedDate, UpdatedDate
                    FROM ct.DonDangKy 
                    WHERE MADDK = @maDDK";

                using (var command = new SqlCommand(sql, connection))
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
                                MaNVXacNhan = reader.GetString("MaNVXacNhan"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                UpdatedDate = reader.GetDateTime("UpdatedDate")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public List<DonDangKyCTModel> GetChiTietVatTuByMaDDK(string maddk)
        {
            var result = new List<DonDangKyCTModel>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT ct.Id, ct.MADDK, ct.MaVTErp, ct.SoLuongHoanUng, ct.CreatedDate,
                           s.SupplyName as TenVT, s.Unit as DVT
                    FROM ct.DonDangKyCT ct
                    LEFT JOIN Supplies s ON ct.MaVTErp = s.ErpId
                    WHERE ct.MADDK = @maDDK
                    ORDER BY ct.CreatedDate";

                using (var command = new SqlCommand(sql, connection))
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
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                TenVT = reader.IsDBNull("TenVT") ? "" : reader.GetString("TenVT"),
                                DVT = reader.IsDBNull("DVT") ? "" : reader.GetString("DVT")
                            });
                        }
                    }
                }
            }

            return result;
        }

        public bool UpdateHoanUngDonDangKy(string maddk, string nguoiXacNhan)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Bước 0: Lấy thông tin đơn đăng ký để kiểm tra và lấy mã nhân viên xây lắp
                        var donDangKy = GetDonDangKyByMa(maddk, connection, transaction);
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
                        int warehouseId = GetWarehouseIdByStaffCode(donDangKy.MaNhanVienXayLap, connection, transaction);
                        if (warehouseId == 0)
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
                            command.Parameters.AddWithValue("@maKhoNguon", warehouseId); // Kho của nhân viên xây lắp
                            command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng vật tư cho đơn đăng ký: {maddk}");
                            command.Parameters.AddWithValue("@createdBy", nguoiXacNhan);
                            command.Parameters.AddWithValue("@entityHoanUng", maddk);

                            transactionId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Bước 3: Tạo transaction chi tiết và cập nhật tồn kho
                        var chiTietList = GetChiTietVatTuByMaDDK(maddk, connection, transaction);
                        foreach (var chiTiet in chiTietList)
                        {
                            if (chiTiet.SoLuongHoanUng > 0)
                            {
                                // Insert transaction detail
                                string insertDetailSql = @"
                                    INSERT INTO TransactionDetails (TransactionId, ErpId, SoLuong, GhiChu)
                                    VALUES (@transactionId, @erpId, @soLuong, @ghiChu)";

                                using (var command = new SqlCommand(insertDetailSql, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@transactionId", transactionId);
                                    command.Parameters.AddWithValue("@erpId", chiTiet.MaVTErp);
                                    command.Parameters.AddWithValue("@soLuong", chiTiet.SoLuongHoanUng);
                                    command.Parameters.AddWithValue("@ghiChu", $"Hoàn ứng: {chiTiet.TenVT}");

                                    command.ExecuteNonQuery();
                                }

                                // Cập nhật tồn kho từ kho của nhân viên xây lắp
                                CapNhatTonKhoHoanUng(connection, transaction, warehouseId, chiTiet.MaVTErp, chiTiet.SoLuongHoanUng);
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw; // Ném lại exception để BLL và GUI có thể hiển thị chi tiết lỗi
                    }
                }
            }
        }

        /// <summary>
        /// Lấy thông tin đơn đăng ký trong transaction
        /// </summary>
        private DonDangKyModel? GetDonDangKyByMa(string maddk, SqlConnection connection, SqlTransaction transaction)
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
                            MaNVXacNhan = reader.GetString("MaNVXacNhan"),
                            CreatedDate = reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.GetDateTime("UpdatedDate")
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tìm warehouse ID dựa trên mã nhân viên xây lắp
        /// </summary>
        private int GetWarehouseIdByStaffCode(string maNhanVien, SqlConnection connection, SqlTransaction transaction)
        {
            string sql = @"
                SELECT Id
                FROM Warehouses
                WHERE MaNV = @maNV";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@maNV", maNhanVien);

                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        private List<DonDangKyCTModel> GetChiTietVatTuByMaDDK(string maddk, SqlConnection connection, SqlTransaction transaction)
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

        /// <summary>
        /// Cập nhật tồn kho khi hoàn ứng từ kho cụ thể
        /// </summary>
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
                int currentStock = result != null ? Convert.ToInt32(result) : 0;

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

        /// <summary>
        /// Tạo số phiếu cho transaction hoàn ứng
        /// </summary>
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

        public List<DonDangKyModel> TimKiemDonDangKy(string? maDDK = null, string? tenKH = null,
            string? nhanVienXayLap = null, bool? trangThai = null)
        {
            var result = new List<DonDangKyModel>();
            var conditions = new List<string>();
            var parameters = new List<SqlParameter>();

            // Xây dựng điều kiện WHERE
            if (!string.IsNullOrEmpty(maDDK))
            {
                conditions.Add("MADDK LIKE @maDDK");
                parameters.Add(new SqlParameter("@maDDK", $"%{maDDK}%"));
            }

            if (!string.IsNullOrEmpty(tenKH))
            {
                conditions.Add("TENKH LIKE @tenKH");
                parameters.Add(new SqlParameter("@tenKH", $"%{tenKH}%"));
            }

            if (!string.IsNullOrEmpty(nhanVienXayLap))
            {
                conditions.Add("NhanVienXayLap LIKE @nhanVienXayLap");
                parameters.Add(new SqlParameter("@nhanVienXayLap", $"%{nhanVienXayLap}%"));
            }

            string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = $@"
                    SELECT Id, MADDK, TENKH, DiaChi, NhanVienKyThuat, 
                           MaNhanVienXayLap, NhanVienXayLap, NgayHoanThanh, 
                           NgayHoanUng, DaHoanUng, ThoiGianXacNhanHoanUng, 
                           MaNVXacNhan, CreatedDate, UpdatedDate
                    FROM ct.DonDangKy 
                    {whereClause}
                    ORDER BY CreatedDate DESC";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

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
                                DaHoanUng = reader.GetBoolean("DaHoanUng"),
                                ThoiGianXacNhanHoanUng = reader.IsDBNull("ThoiGianXacNhanHoanUng") ? null : reader.GetDateTime("ThoiGianXacNhanHoanUng"),
                                MaNVXacNhan = reader.GetString("MaNVXacNhan"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                UpdatedDate = reader.GetDateTime("UpdatedDate")
                            });
                        }
                    }
                }
            }

            return result;
        }

        public DateTime? GetMaxNgayHoanUng()
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

        public List<DonDangKyCTModel> GetChiTietVatTuWithTonKho(string maddk)
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

        public bool IsDonDangKyExists(string maDDK)
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

        public bool InsertDonDangKyWithChiTiet(DonDangKyModel don, List<DonDangKyCTModel> chiTietList)
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
    }
}
