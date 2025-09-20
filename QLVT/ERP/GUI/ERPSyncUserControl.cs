using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLVT.ERP.BLL;
using QLVT.ERP.Models;

namespace QLVT.ERP.GUI
{
    public partial class ERPSyncUserControl : UserControl
    {
        private readonly ERPSyncBLL _syncBLL;
        
        // Controls
        private GroupBox grpConnection;
        private Button btnTestConnection;
        private Label lblConnectionStatus;
        
        private GroupBox grpSyncOptions;
        private DateTimePicker dtpFilterDate;
        private TextBox txtMADDK;
        private TextBox txtMADON;
        private NumericUpDown numSoNghiemThu;
        private NumericUpDown numNamNghiemThu;
        private NumericUpDown numGiaoKhoanID;
        
        private GroupBox grpActions;
        private Button btnSyncDonDangKy;
        private Button btnSyncDonDangKyCT;
        private Button btnSyncSuaChua;
        private Button btnSyncSuaChuaCT;
        private Button btnSyncNghiemThu;
        private Button btnSyncNghiemThuCT;
        private Button btnSyncAll;
        private Button btnGetStats;
        
        private GroupBox grpResults;
        private RichTextBox rtbLog;
        private DataGridView dgvStats;
        
        public ERPSyncUserControl()
        {
            _syncBLL = new ERPSyncBLL();
            InitializeComponent();
            InitializeCustomComponents();
            InitializeData();
        }

        private void InitializeCustomComponents()
        {
            // Main container
            this.Size = new Size(1200, 800);
            this.BackColor = Color.White;
            this.Padding = new Padding(10);

            // Title
            var lblTitle = new Label
            {
                Text = "🔄 ĐỒNG BỘ DỮ LIỆU ERP",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Size = new Size(400, 30),
                Location = new Point(10, 10)
            };
            this.Controls.Add(lblTitle);

            CreateConnectionGroup();
            CreateSyncOptionsGroup();
            CreateActionsGroup();
            CreateResultsGroup();
        }

        private void CreateConnectionGroup()
        {
            grpConnection = new GroupBox
            {
                Text = "🔗 Kết nối ERP",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(380, 80),
                Location = new Point(10, 50)
            };

            btnTestConnection = new Button
            {
                Text = "Test Kết nối",
                Size = new Size(120, 30),
                Location = new Point(10, 25),
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTestConnection.Click += BtnTestConnection_Click;

            lblConnectionStatus = new Label
            {
                Text = "Chưa kiểm tra",
                Size = new Size(200, 30),
                Location = new Point(140, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gray
            };

            grpConnection.Controls.AddRange(new Control[] { btnTestConnection, lblConnectionStatus });
            this.Controls.Add(grpConnection);
        }

        private void CreateSyncOptionsGroup()
        {
            grpSyncOptions = new GroupBox
            {
                Text = "⚙️ Tùy chọn đồng bộ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(380, 200),
                Location = new Point(10, 140)
            };

            // Filter Date
            var lblFilterDate = new Label { Text = "Ngày lọc:", Size = new Size(100, 25), Location = new Point(10, 25) };
            dtpFilterDate = new DateTimePicker
            {
                Size = new Size(200, 25),
                Location = new Point(120, 25),
                Value = new DateTime(2025, 1, 1)
            };

            // MADDK
            var lblMADDK = new Label { Text = "Mã ĐĐK:", Size = new Size(100, 25), Location = new Point(10, 55) };
            txtMADDK = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(120, 55),
                Text = "KH2508.0162"
            };

            // MADON
            var lblMADON = new Label { Text = "Mã đơn SC:", Size = new Size(100, 25), Location = new Point(10, 85) };
            txtMADON = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(120, 85),
                Text = "SC2509.0059"
            };

            // Số nghiệm thu
            var lblSoNghiemThu = new Label { Text = "Số nghiệm thu:", Size = new Size(100, 25), Location = new Point(10, 115) };
            numSoNghiemThu = new NumericUpDown
            {
                Size = new Size(80, 25),
                Location = new Point(120, 115),
                Minimum = 1,
                Maximum = 999999,
                Value = 1468
            };

            // Năm nghiệm thu
            var lblNamNghiemThu = new Label { Text = "Năm:", Size = new Size(40, 25), Location = new Point(210, 115) };
            numNamNghiemThu = new NumericUpDown
            {
                Size = new Size(70, 25),
                Location = new Point(250, 115),
                Minimum = 2020,
                Maximum = 2030,
                Value = 2025
            };

            // Giao khoán ID
            var lblGiaoKhoanID = new Label { Text = "Giao khoán ID:", Size = new Size(100, 25), Location = new Point(10, 145) };
            numGiaoKhoanID = new NumericUpDown
            {
                Size = new Size(100, 25),
                Location = new Point(120, 145),
                Minimum = 1,
                Maximum = 999999,
                Value = 10438
            };

            grpSyncOptions.Controls.AddRange(new Control[] {
                lblFilterDate, dtpFilterDate,
                lblMADDK, txtMADDK,
                lblMADON, txtMADON,
                lblSoNghiemThu, numSoNghiemThu,
                lblNamNghiemThu, numNamNghiemThu,
                lblGiaoKhoanID, numGiaoKhoanID
            });

            this.Controls.Add(grpSyncOptions);
        }

        private void CreateActionsGroup()
        {
            grpActions = new GroupBox
            {
                Text = "🚀 Thao tác đồng bộ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(380, 300),
                Location = new Point(10, 350)
            };

            var buttonStyle = new Action<Button>(btn => {
                btn.Size = new Size(160, 30);
                btn.BackColor = Color.FromArgb(40, 167, 69);
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.Font = new Font("Segoe UI", 9);
            });

            btnSyncDonDangKy = new Button { Text = "📋 Đơn đăng ký", Location = new Point(10, 25) };
            buttonStyle(btnSyncDonDangKy);
            btnSyncDonDangKy.Click += BtnSyncDonDangKy_Click;

            btnSyncDonDangKyCT = new Button { Text = "📋 Chi tiết ĐĐK", Location = new Point(180, 25) };
            buttonStyle(btnSyncDonDangKyCT);
            btnSyncDonDangKyCT.Click += BtnSyncDonDangKyCT_Click;

            btnSyncSuaChua = new Button { Text = "🔧 Sửa chữa", Location = new Point(10, 65) };
            buttonStyle(btnSyncSuaChua);
            btnSyncSuaChua.Click += BtnSyncSuaChua_Click;

            btnSyncSuaChuaCT = new Button { Text = "🔧 Chi tiết SC", Location = new Point(180, 65) };
            buttonStyle(btnSyncSuaChuaCT);
            btnSyncSuaChuaCT.Click += BtnSyncSuaChuaCT_Click;

            btnSyncNghiemThu = new Button { Text = "✅ Nghiệm thu GK", Location = new Point(10, 105) };
            buttonStyle(btnSyncNghiemThu);
            btnSyncNghiemThu.Click += BtnSyncNghiemThu_Click;

            btnSyncNghiemThuCT = new Button { Text = "✅ Chi tiết NTGK", Location = new Point(180, 105) };
            buttonStyle(btnSyncNghiemThuCT);
            btnSyncNghiemThuCT.Click += BtnSyncNghiemThuCT_Click;

            btnSyncAll = new Button 
            { 
                Text = "🔄 ĐỒNG BỘ TẤT CẢ", 
                Location = new Point(10, 155),
                Size = new Size(330, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnSyncAll.Click += BtnSyncAll_Click;

            btnGetStats = new Button 
            { 
                Text = "📊 Xem thống kê", 
                Location = new Point(10, 205),
                Size = new Size(330, 30),
                BackColor = Color.FromArgb(102, 16, 242),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnGetStats.Click += BtnGetStats_Click;

            grpActions.Controls.AddRange(new Control[] {
                btnSyncDonDangKy, btnSyncDonDangKyCT,
                btnSyncSuaChua, btnSyncSuaChuaCT,
                btnSyncNghiemThu, btnSyncNghiemThuCT,
                btnSyncAll, btnGetStats
            });

            this.Controls.Add(grpActions);
        }

        private void CreateResultsGroup()
        {
            grpResults = new GroupBox
            {
                Text = "📋 Kết quả đồng bộ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(780, 600),
                Location = new Point(400, 50)
            };

            // Log area
            var lblLog = new Label
            {
                Text = "📝 Log đồng bộ:",
                Size = new Size(100, 20),
                Location = new Point(10, 25)
            };

            rtbLog = new RichTextBox
            {
                Size = new Size(760, 300),
                Location = new Point(10, 45),
                ReadOnly = true,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = new Font("Consolas", 9),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Stats area
            var lblStats = new Label
            {
                Text = "📊 Thống kê dữ liệu:",
                Size = new Size(150, 20),
                Location = new Point(10, 355)
            };

            dgvStats = new DataGridView
            {
                Size = new Size(760, 230),
                Location = new Point(10, 375),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray
            };

            grpResults.Controls.AddRange(new Control[] { lblLog, rtbLog, lblStats, dgvStats });
            this.Controls.Add(grpResults);
        }

        private void InitializeData()
        {
            LogMessage("🚀 Khởi tạo giao diện đồng bộ ERP thành công.", Color.Blue);
            LogMessage("💡 Hướng dẫn: Kiểm tra kết nối ERP trước khi đồng bộ dữ liệu.", Color.Gray);
        }

        private void LogMessage(string message, Color? color = null)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action(() => LogMessage(message, color)));
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}\n";
            
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color ?? Color.Black;
            rtbLog.AppendText(logEntry);
            rtbLog.ScrollToCaret();
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            lblConnectionStatus.Text = "Đang kiểm tra...";
            lblConnectionStatus.ForeColor = Color.Orange;
            btnTestConnection.Enabled = false;

            try
            {
                var isConnected = await _syncBLL.TestERPConnectionAsync();
                
                if (isConnected)
                {
                    lblConnectionStatus.Text = "✅ Kết nối thành công";
                    lblConnectionStatus.ForeColor = Color.Green;
                    LogMessage("✅ Kết nối ERP thành công!", Color.Green);
                }
                else
                {
                    lblConnectionStatus.Text = "❌ Kết nối thất bại";
                    lblConnectionStatus.ForeColor = Color.Red;
                    LogMessage("❌ Không thể kết nối đến ERP. Kiểm tra lại cấu hình.", Color.Red);
                }
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = "❌ Lỗi kết nối";
                lblConnectionStatus.ForeColor = Color.Red;
                LogMessage($"❌ Lỗi kết nối ERP: {ex.Message}", Color.Red);
            }
            finally
            {
                btnTestConnection.Enabled = true;
            }
        }

        private async void BtnSyncDonDangKy_Click(object sender, EventArgs e)
        {
            btnSyncDonDangKy.Enabled = false;
            LogMessage("🔄 Bắt đầu đồng bộ Đơn đăng ký...", Color.Blue);

            try
            {
                var result = await _syncBLL.SyncDonDangKyAsync(dtpFilterDate.Value);
                
                if (result.Success)
                {
                    LogMessage($"✅ {result.Message}", Color.Green);
                }
                else
                {
                    LogMessage($"❌ {result.Message}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi: {ex.Message}", Color.Red);
            }
            finally
            {
                btnSyncDonDangKy.Enabled = true;
            }
        }

        private async void BtnSyncDonDangKyCT_Click(object sender, EventArgs e)
        {
            btnSyncDonDangKyCT.Enabled = false;
            LogMessage("🔄 Bắt đầu đồng bộ Chi tiết đơn đăng ký...", Color.Blue);

            try
            {
                var result = await _syncBLL.SyncDonDangKyCTAsync(txtMADDK.Text);
                
                if (result.Success)
                {
                    LogMessage($"✅ {result.Message}", Color.Green);
                }
                else
                {
                    LogMessage($"❌ {result.Message}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi: {ex.Message}", Color.Red);
            }
            finally
            {
                btnSyncDonDangKyCT.Enabled = true;
            }
        }

        private async void BtnSyncSuaChua_Click(object sender, EventArgs e)
        {
            btnSyncSuaChua.Enabled = false;
            LogMessage("🔄 Bắt đầu đồng bộ Sửa chữa...", Color.Blue);

            try
            {
                var result = await _syncBLL.SyncSuaChuaAsync(dtpFilterDate.Value);
                
                if (result.Success)
                {
                    LogMessage($"✅ {result.Message}", Color.Green);
                }
                else
                {
                    LogMessage($"❌ {result.Message}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi: {ex.Message}", Color.Red);
            }
            finally
            {
                btnSyncSuaChua.Enabled = true;
            }
        }

        private async void BtnSyncSuaChuaCT_Click(object sender, EventArgs e)
        {
            btnSyncSuaChuaCT.Enabled = false;
            LogMessage("🔄 Bắt đầu đồng bộ Chi tiết sửa chữa...", Color.Blue);

            try
            {
                var result = await _syncBLL.SyncSuaChuaCTAsync(txtMADON.Text);
                
                if (result.Success)
                {
                    LogMessage($"✅ {result.Message}", Color.Green);
                }
                else
                {
                    LogMessage($"❌ {result.Message}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi: {ex.Message}", Color.Red);
            }
            finally
            {
                btnSyncSuaChuaCT.Enabled = true;
            }
        }

        private async void BtnSyncNghiemThu_Click(object sender, EventArgs e)
        {
            btnSyncNghiemThu.Enabled = false;
            LogMessage("🔄 Bắt đầu đồng bộ Nghiệm thu giao khoán...", Color.Blue);

            try
            {
                var result = await _syncBLL.SyncNghiemThuGiaoKhoanAsync((int)numSoNghiemThu.Value, (int)numNamNghiemThu.Value);
                
                if (result.Success)
                {
                    LogMessage($"✅ {result.Message}", Color.Green);
                }
                else
                {
                    LogMessage($"❌ {result.Message}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi: {ex.Message}", Color.Red);
            }
            finally
            {
                btnSyncNghiemThu.Enabled = true;
            }
        }

        private async void BtnSyncNghiemThuCT_Click(object sender, EventArgs e)
        {
            btnSyncNghiemThuCT.Enabled = false;
            LogMessage("🔄 Bắt đầu đồng bộ Chi tiết nghiệm thu giao khoán...", Color.Blue);

            try
            {
                var result = await _syncBLL.SyncNghiemThuGiaoKhoanCTAsync((long)numGiaoKhoanID.Value);
                
                if (result.Success)
                {
                    LogMessage($"✅ {result.Message}", Color.Green);
                }
                else
                {
                    LogMessage($"❌ {result.Message}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi: {ex.Message}", Color.Red);
            }
            finally
            {
                btnSyncNghiemThuCT.Enabled = true;
            }
        }

        private async void BtnSyncAll_Click(object sender, EventArgs e)
        {
            btnSyncAll.Enabled = false;
            LogMessage("🚀 BẮT ĐẦU ĐỒNG BỘ TẤT CẢ DỮ LIỆU ERP...", Color.Purple);
            LogMessage("⏰ Quá trình này có thể mất vài phút...", Color.Orange);

            try
            {
                var results = await _syncBLL.SyncAllDataAsync(
                    dtpFilterDate.Value,
                    txtMADDK.Text,
                    txtMADON.Text,
                    (int)numSoNghiemThu.Value,
                    (int)numNamNghiemThu.Value,
                    (long)numGiaoKhoanID.Value
                );

                LogMessage("📊 KẾT QUẢ ĐỒNG BỘ TỔNG QUAN:", Color.Purple);
                var totalSuccess = 0;
                var totalRecords = 0;

                foreach (var result in results)
                {
                    if (result.Success)
                    {
                        totalSuccess++;
                        totalRecords += result.RecordsAffected;
                        LogMessage($"✅ {result.Message}", Color.Green);
                    }
                    else
                    {
                        LogMessage($"❌ {result.Message}", Color.Red);
                    }
                }

                LogMessage($"🎉 HOÀN TẤT: {totalSuccess}/{results.Count} bảng thành công, tổng {totalRecords} bản ghi.", Color.Purple);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi tổng quan: {ex.Message}", Color.Red);
            }
            finally
            {
                btnSyncAll.Enabled = true;
            }
        }

        private async void BtnGetStats_Click(object sender, EventArgs e)
        {
            btnGetStats.Enabled = false;
            LogMessage("📊 Đang lấy thống kê dữ liệu...", Color.Blue);

            try
            {
                var stats = await _syncBLL.GetERPDataStatsAsync();
                
                // Setup DataGridView
                dgvStats.DataSource = null;
                dgvStats.Rows.Clear();
                dgvStats.Columns.Clear();

                dgvStats.Columns.Add("TableName", "Tên bảng");
                dgvStats.Columns.Add("RecordCount", "Số bản ghi");
                dgvStats.Columns[0].Width = 300;
                dgvStats.Columns[1].Width = 100;

                foreach (var stat in stats)
                {
                    dgvStats.Rows.Add($"ct.{stat.Key}", stat.Value.ToString("N0"));
                }

                var totalRecords = stats.Values.Sum();
                LogMessage($"📊 Thống kê: {stats.Count} bảng, tổng {totalRecords:N0} bản ghi.", Color.Blue);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Lỗi lấy thống kê: {ex.Message}", Color.Red);
            }
            finally
            {
                btnGetStats.Enabled = true;
            }
        }
    }
}