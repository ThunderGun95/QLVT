using System;
using System.Windows.Forms;
using QLVT.GUI;

namespace QLVT.Test
{
    public partial class TestHoanUngMC4BatchForm : Form
    {
        public TestHoanUngMC4BatchForm()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            this.Load += TestHoanUngMC4BatchForm_Load;
        }

        private void TestHoanUngMC4BatchForm_Load(object? sender, EventArgs e)
        {
            try
            {
                var hoanUngBatchUC = new HoanUngMC4BatchUserControl();
                hoanUngBatchUC.Dock = DockStyle.Fill;
                this.Controls.Add(hoanUngBatchUC);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo UserControl: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TestHoanUngMC4BatchForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1450, 750);
            this.Name = "TestHoanUngMC4BatchForm";
            this.Text = "Test - Hoàn Ứng MC4 Hàng Loạt";
            this.WindowState = FormWindowState.Maximized;
            this.ResumeLayout(false);
        }
    }
}
