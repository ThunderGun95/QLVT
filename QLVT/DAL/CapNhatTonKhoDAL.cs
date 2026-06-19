using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.DAL
{
    public class CapNhatTonKhoDAL
    {
        private const decimal Tolerance = 0.004m;

        public List<TonKhoRebuildItem> GetChenhLechTonKho(bool includeZeroInventoryWithoutTransactions)
        {
            var result = new List<TonKhoRebuildItem>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(BuildPreviewSql(includeZeroInventoryWithoutTransactions), connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tonHienTai = Convert.ToDecimal(reader["TonHienTai"]);
                        var tonTinhLai = Convert.ToDecimal(reader["TonTinhLai"]);

                        result.Add(new TonKhoRebuildItem
                        {
                            STT = result.Count + 1,
                            WarehouseId = Convert.ToInt64(reader["WarehouseId"]),
                            SupplyErpId = Convert.ToInt64(reader["SupplyErpId"]),
                            MaKho = reader["MaKho"]?.ToString() ?? string.Empty,
                            TenKho = reader["TenKho"]?.ToString() ?? string.Empty,
                            MaVatTu = reader["MaVatTu"]?.ToString() ?? string.Empty,
                            TenVatTu = reader["TenVatTu"]?.ToString() ?? string.Empty,
                            SoLuongNhap = Convert.ToDecimal(reader["SoLuongNhap"]),
                            SoLuongXuat = Convert.ToDecimal(reader["SoLuongXuat"]),
                            TonHienTai = tonHienTai,
                            TonTinhLai = tonTinhLai,
                            ChenhLech = tonTinhLai - tonHienTai,
                            TrangThai = reader["TrangThai"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }

            return result;
        }

        public TonKhoRebuildResult RebuildTonKho(bool zeroInventoryWithoutTransactions)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = ExecuteRebuildBatch(connection, transaction, zeroInventoryWithoutTransactions);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static string BuildPreviewSql(bool includeZeroInventoryWithoutTransactions)
        {
            var orphanFilter = includeZeroInventoryWithoutTransactions
                ? string.Empty
                : " AND c.WarehouseId IS NOT NULL";

            return $@"
                WITH Movement AS (
                    SELECT
                        td.MaKhoNhap AS WarehouseId,
                        td.ErpId AS SupplyErpId,
                        SUM(CAST(td.SoLuong AS decimal(18, 2))) AS SoLuongNhap,
                        CAST(0 AS decimal(18, 2)) AS SoLuongXuat
                    FROM Transactions t
                    INNER JOIN TransactionDetails td ON td.TransactionId = t.Id
                    WHERE ISNULL(t.IsDeleted, 0) = 0
                      AND ISNULL(td.IsDeleted, 0) = 0
                      AND td.MaKhoNhap IS NOT NULL
                    GROUP BY td.MaKhoNhap, td.ErpId

                    UNION ALL

                    SELECT
                        td.MaKhoXuat AS WarehouseId,
                        td.ErpId AS SupplyErpId,
                        CAST(0 AS decimal(18, 2)) AS SoLuongNhap,
                        SUM(CAST(td.SoLuong AS decimal(18, 2))) AS SoLuongXuat
                    FROM Transactions t
                    INNER JOIN TransactionDetails td ON td.TransactionId = t.Id
                    WHERE ISNULL(t.IsDeleted, 0) = 0
                      AND ISNULL(td.IsDeleted, 0) = 0
                      AND td.MaKhoXuat IS NOT NULL
                    GROUP BY td.MaKhoXuat, td.ErpId
                ),
                Calculated AS (
                    SELECT
                        WarehouseId,
                        SupplyErpId,
                        SUM(SoLuongNhap) AS SoLuongNhap,
                        SUM(SoLuongXuat) AS SoLuongXuat,
                        SUM(SoLuongNhap - SoLuongXuat) AS SoLuongTon
                    FROM Movement
                    GROUP BY WarehouseId, SupplyErpId
                )
                SELECT
                    COALESCE(i.WarehouseId, c.WarehouseId) AS WarehouseId,
                    COALESCE(i.SupplyErpId, c.SupplyErpId) AS SupplyErpId,
                    ISNULL(w.MaKho, '') AS MaKho,
                    ISNULL(w.TenKho, '') AS TenKho,
                    ISNULL(vt.Code, CAST(COALESCE(i.SupplyErpId, c.SupplyErpId) AS varchar(30))) AS MaVatTu,
                    ISNULL(vt.TenVatTu, '') AS TenVatTu,
                    ISNULL(c.SoLuongNhap, 0) AS SoLuongNhap,
                    ISNULL(c.SoLuongXuat, 0) AS SoLuongXuat,
                    ISNULL(i.SoLuongTon, 0) AS TonHienTai,
                    ISNULL(c.SoLuongTon, 0) AS TonTinhLai,
                    CASE
                        WHEN i.Id IS NULL THEN N'Thiếu dòng tồn kho'
                        WHEN c.WarehouseId IS NULL THEN N'Không có giao dịch'
                        ELSE N'Lệch tồn'
                    END AS TrangThai
                FROM Inventory i
                FULL OUTER JOIN Calculated c
                    ON c.WarehouseId = i.WarehouseId
                   AND c.SupplyErpId = i.SupplyErpId
                LEFT JOIN Warehouses w ON w.Id = COALESCE(i.WarehouseId, c.WarehouseId)
                LEFT JOIN ViewVatTus vt ON vt.ErpId = COALESCE(i.SupplyErpId, c.SupplyErpId)
                WHERE ABS(ISNULL(i.SoLuongTon, 0) - ISNULL(c.SoLuongTon, 0)) > {Tolerance.ToString(System.Globalization.CultureInfo.InvariantCulture)}
                  {orphanFilter}
                ORDER BY TenKho, MaVatTu";
        }

        private static void CreateCalculatedInventoryTable(SqlConnection connection, SqlTransaction transaction)
        {
            var sql = @"
                CREATE TABLE #CalculatedInventory
                (
                    WarehouseId bigint NOT NULL,
                    SupplyErpId bigint NOT NULL,
                    SoLuongNhap decimal(18, 2) NOT NULL,
                    SoLuongXuat decimal(18, 2) NOT NULL,
                    SoLuongTon decimal(18, 2) NOT NULL,
                    PRIMARY KEY (WarehouseId, SupplyErpId)
                );

                WITH Movement AS (
                    SELECT
                        td.MaKhoNhap AS WarehouseId,
                        td.ErpId AS SupplyErpId,
                        SUM(CAST(td.SoLuong AS decimal(18, 2))) AS SoLuongNhap,
                        CAST(0 AS decimal(18, 2)) AS SoLuongXuat
                    FROM Transactions t
                    INNER JOIN TransactionDetails td ON td.TransactionId = t.Id
                    WHERE ISNULL(t.IsDeleted, 0) = 0
                      AND ISNULL(td.IsDeleted, 0) = 0
                      AND td.MaKhoNhap IS NOT NULL
                    GROUP BY td.MaKhoNhap, td.ErpId

                    UNION ALL

                    SELECT
                        td.MaKhoXuat AS WarehouseId,
                        td.ErpId AS SupplyErpId,
                        CAST(0 AS decimal(18, 2)) AS SoLuongNhap,
                        SUM(CAST(td.SoLuong AS decimal(18, 2))) AS SoLuongXuat
                    FROM Transactions t
                    INNER JOIN TransactionDetails td ON td.TransactionId = t.Id
                    WHERE ISNULL(t.IsDeleted, 0) = 0
                      AND ISNULL(td.IsDeleted, 0) = 0
                      AND td.MaKhoXuat IS NOT NULL
                    GROUP BY td.MaKhoXuat, td.ErpId
                )
                INSERT INTO #CalculatedInventory (WarehouseId, SupplyErpId, SoLuongNhap, SoLuongXuat, SoLuongTon)
                SELECT
                    WarehouseId,
                    SupplyErpId,
                    SUM(SoLuongNhap) AS SoLuongNhap,
                    SUM(SoLuongXuat) AS SoLuongXuat,
                    SUM(SoLuongNhap - SoLuongXuat) AS SoLuongTon
                FROM Movement
                GROUP BY WarehouseId, SupplyErpId";

            ExecuteNonQuery(connection, transaction, sql);
        }

        private static TonKhoRebuildResult ExecuteRebuildBatch(SqlConnection connection, SqlTransaction transaction, bool zeroInventoryWithoutTransactions)
        {
            var sql = @"
                CREATE TABLE #CalculatedInventory
                (
                    WarehouseId bigint NOT NULL,
                    SupplyErpId bigint NOT NULL,
                    SoLuongNhap decimal(18, 2) NOT NULL,
                    SoLuongXuat decimal(18, 2) NOT NULL,
                    SoLuongTon decimal(18, 2) NOT NULL,
                    PRIMARY KEY (WarehouseId, SupplyErpId)
                );

                WITH Movement AS (
                    SELECT
                        td.MaKhoNhap AS WarehouseId,
                        td.ErpId AS SupplyErpId,
                        SUM(CAST(td.SoLuong AS decimal(18, 2))) AS SoLuongNhap,
                        CAST(0 AS decimal(18, 2)) AS SoLuongXuat
                    FROM Transactions t
                    INNER JOIN TransactionDetails td ON td.TransactionId = t.Id
                    WHERE ISNULL(t.IsDeleted, 0) = 0
                      AND ISNULL(td.IsDeleted, 0) = 0
                      AND td.MaKhoNhap IS NOT NULL
                    GROUP BY td.MaKhoNhap, td.ErpId

                    UNION ALL

                    SELECT
                        td.MaKhoXuat AS WarehouseId,
                        td.ErpId AS SupplyErpId,
                        CAST(0 AS decimal(18, 2)) AS SoLuongNhap,
                        SUM(CAST(td.SoLuong AS decimal(18, 2))) AS SoLuongXuat
                    FROM Transactions t
                    INNER JOIN TransactionDetails td ON td.TransactionId = t.Id
                    WHERE ISNULL(t.IsDeleted, 0) = 0
                      AND ISNULL(td.IsDeleted, 0) = 0
                      AND td.MaKhoXuat IS NOT NULL
                    GROUP BY td.MaKhoXuat, td.ErpId
                )
                INSERT INTO #CalculatedInventory (WarehouseId, SupplyErpId, SoLuongNhap, SoLuongXuat, SoLuongTon)
                SELECT
                    WarehouseId,
                    SupplyErpId,
                    SUM(SoLuongNhap) AS SoLuongNhap,
                    SUM(SoLuongXuat) AS SoLuongXuat,
                    SUM(SoLuongNhap - SoLuongXuat) AS SoLuongTon
                FROM Movement
                GROUP BY WarehouseId, SupplyErpId;

                DECLARE @RowsUpdated int = 0;
                DECLARE @RowsInserted int = 0;
                DECLARE @RowsZeroed int = 0;

                UPDATE i
                SET i.SoLuongTon = c.SoLuongTon,
                    i.LastUpdated = GETDATE()
                FROM Inventory i
                INNER JOIN #CalculatedInventory c
                    ON c.WarehouseId = i.WarehouseId
                   AND c.SupplyErpId = i.SupplyErpId
                WHERE ABS(ISNULL(i.SoLuongTon, 0) - ISNULL(c.SoLuongTon, 0)) > @Tolerance;

                SET @RowsUpdated = @@ROWCOUNT;

                INSERT INTO Inventory (WarehouseId, SupplyErpId, SoLuongTon, LastUpdated)
                SELECT c.WarehouseId, c.SupplyErpId, c.SoLuongTon, GETDATE()
                FROM #CalculatedInventory c
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM Inventory i
                    WHERE i.WarehouseId = c.WarehouseId
                      AND i.SupplyErpId = c.SupplyErpId
                );

                SET @RowsInserted = @@ROWCOUNT;

                IF @ZeroInventoryWithoutTransactions = 1
                BEGIN
                    UPDATE i
                    SET i.SoLuongTon = 0,
                        i.LastUpdated = GETDATE()
                    FROM Inventory i
                    WHERE ABS(ISNULL(i.SoLuongTon, 0)) > @Tolerance
                      AND NOT EXISTS (
                          SELECT 1
                          FROM #CalculatedInventory c
                          WHERE c.WarehouseId = i.WarehouseId
                            AND c.SupplyErpId = i.SupplyErpId
                      );

                    SET @RowsZeroed = @@ROWCOUNT;
                END

                SELECT @RowsUpdated AS RowsUpdated,
                       @RowsInserted AS RowsInserted,
                       @RowsZeroed AS RowsZeroed;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@Tolerance", Tolerance);
                command.Parameters.AddWithValue("@ZeroInventoryWithoutTransactions", zeroInventoryWithoutTransactions);
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception("Không nhận được kết quả cập nhật tồn kho");
                    }

                    return new TonKhoRebuildResult
                    {
                        RowsUpdated = Convert.ToInt32(reader["RowsUpdated"]),
                        RowsInserted = Convert.ToInt32(reader["RowsInserted"]),
                        RowsZeroed = Convert.ToInt32(reader["RowsZeroed"]),
                        UpdatedAt = DateTime.Now
                    };
                }
            }
        }

        private static int ExecuteNonQuery(SqlConnection connection, SqlTransaction transaction, string sql)
        {
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@Tolerance", Tolerance);
                return command.ExecuteNonQuery();
            }
        }
    }
}
