using QLVT.BLL;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class MainForm : Form
    {
        private readonly MenuBLL menuBLL;
        private readonly AuthenticationBLL authBLL;

        public MainForm()
        {
            InitializeComponent();
            menuBLL = new MenuBLL();
            authBLL = new AuthenticationBLL();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Initialize form
            InitializeForm();
            
            // Load user menus
            LoadUserMenus();
            
            // Update status bar
            UpdateStatusBar();
            
            // Start timer for time display
            timer.Start();
        }

        private void InitializeForm()
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();
            if (currentUser == null)
            {
                MessageBox.Show("Phiên đăng nhập không hợp lệ!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Set form title with user info
            this.Text = $"QLVT - Hệ thống quản lý vật tư - Xin chào: {currentUser.FullName}";
        }

        private void LoadUserMenus()
        {
            try
            {
                // Get user menus
                var userMenus = menuBLL.GetCurrentUserMenus();
                
                // Note: Don't clear existing menus as we have "Tác vụ" menu in designer
                // Just add additional user menus if any
                
                // Build additional menu items
                BuildMenuStrip(userMenus);
                
                lblStatus.Text = "Menu đã được tải thành công";
            }
            catch (Exception ex)
            {
                // Don't show error for now, just log it
                lblStatus.Text = "Đã tải menu cơ bản";
            }
        }

        private void BuildMenuStrip(List<Menu> menus)
        {
            foreach (var menu in menus)
            {
                var menuItem = CreateMenuItem(menu);
                menuStrip.Items.Add(menuItem);
            }
        }

        private ToolStripMenuItem CreateMenuItem(Menu menu)
        {
            var menuItem = new ToolStripMenuItem(menu.MenuName)
            {
                Tag = menu,
                Name = $"menu_{menu.MenuID}"
            };

            // Add event handler for menu click
            if (!string.IsNullOrEmpty(menu.FormName))
            {
                menuItem.Click += MenuItem_Click;
            }

            // Add sub-menus
            foreach (var subMenu in menu.SubMenus)
            {
                var subMenuItem = CreateMenuItem(subMenu);
                menuItem.DropDownItems.Add(subMenuItem);
            }

            return menuItem;
        }

        private void MenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is Menu menu)
            {
                HandleMenuClick(menu);
            }
        }

        private void HandleMenuClick(Menu menu)
        {
            try
            {
                lblStatus.Text = $"Đang mở: {menu.MenuName}...";

                // Check if user has permission
                if (!menuBLL.HasMenuAccess(menu.MenuID))
                {
                    MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblStatus.Text = "Không có quyền truy cập";
                    return;
                }

                // Handle specific menu actions
                switch (menu.FormName)
                {
                    case "UserProfileForm":
                        ShowUserProfile();
                        break;
                    case "ChangePasswordForm":
                        ShowChangePassword();
                        break;
                    case "UserManagementForm":
                        ShowMessage("Chức năng quản lý người dùng sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "PermissionForm":
                        ShowMessage("Chức năng phân quyền sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "LogoutAction":
                        PerformLogout();
                        break;
                    case "MaterialManagementForm":
                        ShowMessage("Chức năng quản lý vật tư sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "WarehouseManagementForm":
                        ShowMessage("Chức năng quản lý kho sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "ImportForm":
                        ShowMessage("Chức năng nhập kho sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "ExportForm":
                        ShowMessage("Chức năng xuất kho sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "InventoryReportForm":
                        ShowMessage("Chức năng báo cáo tồn kho sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "TransactionReportForm":
                        ShowMessage("Chức năng báo cáo xuất nhập sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "BackupForm":
                        ShowMessage("Chức năng sao lưu dữ liệu sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "SettingsForm":
                        ShowMessage("Chức năng cài đặt hệ thống sẽ được phát triển trong phiên bản tiếp theo.");
                        break;
                    case "UnitsForm":
                        ShowUnitsControl();
                        break;
                    case "ManufacturersForm":
                        ShowManufacturersControl();
                        break;
                    case "SuppliesForm":
                        ShowSuppliesControl();
                        break;
                    case "DepartmentsForm":
                        ShowDepartmentsControl();
                        break;
                    case "StaffsForm":
                        ShowStaffsControl();
                        break;
                    default:
                        ShowMessage($"Chức năng '{menu.MenuName}' chưa được triển khai.");
                        break;
                }

                lblStatus.Text = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chức năng: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi";
            }
        }

        private void ShowUserProfile()
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();
            if (currentUser != null)
            {
                var roles = authBLL.GetCurrentUserRoles();
                string roleNames = string.Join(", ", roles.Select(r => r.RoleName));
                
                MessageBox.Show(
                    $"Thông tin tài khoản:\n\n" +
                    $"Tên đăng nhập: {currentUser.Username}\n" +
                    $"Họ tên: {currentUser.FullName}\n" +
                    $"Vai trò: {roleNames}\n" +
                    $"Ngày tạo: {currentUser.CreatedDate:dd/MM/yyyy}\n" +
                    $"Lần đăng nhập cuối: {currentUser.LastLogin?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa có"}",
                    "Thông tin tài khoản",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void ShowChangePassword()
        {
            var changePasswordForm = new ChangePasswordForm();
            changePasswordForm.ShowDialog();
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowUnitsControl()
        {
            try
            {
                // Clear existing controls
                pnlMain.Controls.Clear();
                
                // Create and show Units UserControl
                var unitsControl = new UnitsUserControl();
                unitsControl.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(unitsControl);
                
                lblStatus.Text = "Đang hiển thị danh sách đơn vị tính";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách đơn vị tính: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi";
            }
        }

        private void ShowManufacturersControl()
        {
            try
            {
                // Clear existing controls
                pnlMain.Controls.Clear();
                
                // Create and show Manufacturers UserControl
                var manufacturersControl = new ManufacturersUserControl();
                manufacturersControl.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(manufacturersControl);
                
                lblStatus.Text = "Đang hiển thị danh sách nhà sản xuất";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách nhà sản xuất: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi";
            }
        }

        private void ShowSuppliesControl()
        {
            try
            {
                // Clear existing controls
                pnlMain.Controls.Clear();
                
                // Create and show Supplies UserControl
                var suppliesControl = new SuppliesUserControl();
                suppliesControl.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(suppliesControl);
                
                lblStatus.Text = "Đang hiển thị danh sách vật tư";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách vật tư: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi";
            }
        }

        private void ShowDepartmentsControl()
        {
            try
            {
                // Clear existing controls
                pnlMain.Controls.Clear();
                
                // Create and show Departments UserControl
                var departmentsControl = new DepartmentsUserControl();
                departmentsControl.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(departmentsControl);
                
                lblStatus.Text = "Đang hiển thị danh sách phòng ban";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách phòng ban: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi";
            }
        }

        private void ShowStaffsControl()
        {
            try
            {
                // Clear existing controls
                pnlMain.Controls.Clear();
                
                // Create and show Staffs UserControl
                var staffsControl = new StaffsUserControl();
                staffsControl.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(staffsControl);
                
                lblStatus.Text = "Đang hiển thị danh sách nhân viên";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách nhân viên: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Lỗi";
            }
        }

        private void PerformLogout()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                authBLL.Logout();
                this.Close();
            }
        }

        private void UpdateStatusBar()
        {
            var currentUser = AuthenticationBLL.GetCurrentUser();
            if (currentUser != null)
            {
                lblUser.Text = $"Người dùng: {currentUser.FullName}";
            }
            
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            
            // Show login form again
            Application.OpenForms.OfType<LoginForm>().FirstOrDefault()?.Show();
        }

        #region Menu Event Handlers

        private void mnuNhapTonDauKy_Click(object sender, EventArgs e)
        {
            LoadUserControl(new OpeningInventoryUserControl(), "Nhập tồn đầu kỳ");
        }

        private void mnuNhapKho_Click(object sender, EventArgs e)
        {
            LoadUserControl(new ImportTaskUserControl(), "Nhập kho vật tư");
        }

        private void mnuXuatKho_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xuất kho đang được phát triển!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuTraKho_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng trả kho đang được phát triển!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuHoanUng_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng hoàn ứng đang được phát triển!", "Thông báo", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region UserControl Management

        private void LoadUserControl(UserControl userControl, string title)
        {
            try
            {
                // Clear main panel
                pnlMain.Controls.Clear();
                
                // Configure UserControl
                userControl.Dock = DockStyle.Fill;
                
                // Add to main panel
                pnlMain.Controls.Add(userControl);
                
                // Update status
                lblStatus.Text = $"Đã mở: {title}";
                
                // Update form title
                this.Text = $"QLVT - {title}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở {title}: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Lỗi khi mở {title}";
            }
        }

        #endregion
    }
}
