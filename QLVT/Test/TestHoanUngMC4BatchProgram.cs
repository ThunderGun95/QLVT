using System;
using System.Windows.Forms;
using QLVT.GUI;

namespace QLVT.Test
{
    internal class TestHoanUngMC4BatchProgram
    {
        [STAThread]
        static void MainTest()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                var testForm = new TestHoanUngMC4BatchForm();
                Application.Run(testForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi động ứng dụng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}