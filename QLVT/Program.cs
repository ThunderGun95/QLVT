using QLVT.GUI;
using QLVT.Utils;

namespace QLVT;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        // Test database connection before starting
        if (!DatabaseHelper.TestConnection())
        {
            MessageBox.Show(
                "Không thể kết nối đến database QLVT_DB!\n\n" +
                "Vui lòng:\n" +
                "1. Đảm bảo SQL Server đang chạy\n" +
                "2. Chạy script CreateDatabase.sql để tạo database\n" +
                "3. Kiểm tra connection string trong DatabaseHelper.cs",
                "Lỗi kết nối Database",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }
        
        Application.Run(new LoginForm());
    }    
}