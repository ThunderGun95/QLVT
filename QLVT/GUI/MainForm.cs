using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.BLL;
using QLVT.Models;
using QLVT.Utils;

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
                // Xóa tất cả menu hiện tại trước
                menuStrip.Items.Clear();
                
                // Get user menus from database
                System.Diagnostics.Debug.WriteLine("MainForm: Loading user menus from database...");
                var userMenus = menuBLL.GetCurrentUserMenus();
                
                if (userMenus == null || !userMenus.Any())
                {
                    System.Diagnostics.Debug.WriteLine("MainForm: No menus from database, creating basic menu");
                    // Nếu không có menu từ database, tạo menu cơ bản
                    CreateBasicMenu();
                    lblStatus.Text = "Đã tải menu cơ bản";
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"MainForm: Found {userMenus.Count} root menus from database");
                // Build menu từ database
                BuildMenuStrip(userMenus);
                
                lblStatus.Text = "Menu đã được tải từ cơ sở dữ liệu";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainForm: Error loading menus: {ex.Message}");
                // Fallback to basic menu if database fails
                CreateBasicMenu();
                lblStatus.Text = "Đã tải menu cơ bản (lỗi database)";
                System.Diagnostics.Debug.WriteLine($"Lỗi load menu: {ex.Message}");
            }
        }

        private void CreateBasicMenu()
        {
            // Tạo menu cơ bản nếu không load được từ database
            var tacVuMenu = new ToolStripMenuItem("Tác vụ");
            tacVuMenu.DropDownItems.Add(new ToolStripMenuItem("📋 Nhập tồn đầu kỳ", null, (s, e) => 
                FormFactory.OpenUserControl("OpeningInventoryUserControl", pnlMain, "Nhập tồn đầu kỳ", UpdateStatusText)));
            tacVuMenu.DropDownItems.Add(new ToolStripMenuItem("📦 Nhập kho vật tư", null, (s, e) => 
                FormFactory.OpenUserControl("NhapKhoErpTaskUserControl", pnlMain, "Nhập kho vật tư", UpdateStatusText)));
            tacVuMenu.DropDownItems.Add(new ToolStripMenuItem("📤 Xuất kho vật tư", null, (s, e) => 
                FormFactory.OpenUserControl("XuatKhoErpTaskUserControl", pnlMain, "Xuất kho vật tư", UpdateStatusText)));
            tacVuMenu.DropDownItems.Add(new ToolStripMenuItem("↩️ Hoàn ứng BGK", null, (s, e) => 
                FormFactory.OpenUserControl("HoanUngBGKUserControl", pnlMain, "Hoàn ứng BGK", UpdateStatusText)));
            
            var baoCaoMenu = new ToolStripMenuItem("Báo cáo");
            baoCaoMenu.DropDownItems.Add(new ToolStripMenuItem("📊 Báo cáo tồn kho", null, (s, e) => 
                FormFactory.OpenUserControl("BaoCaoTonKhoUserControl", pnlMain, "Báo cáo tồn kho", UpdateStatusText)));
            baoCaoMenu.DropDownItems.Add(new ToolStripMenuItem("📊 Báo cáo xuất nhập tồn", null, (s, e) => 
                FormFactory.OpenUserControl("BaoCaoXuatNhapTonUserControl", pnlMain, "Báo cáo xuất nhập tồn", UpdateStatusText)));
            
            menuStrip.Items.Add(tacVuMenu);
            menuStrip.Items.Add(baoCaoMenu);
        }

        private void UpdateStatusText(string text)
        {
            lblStatus.Text = text;
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
            var menuItem = new ToolStripMenuItem($"{menu.MenuIcon} {menu.MenuName}")
            {
                Tag = menu,
                Name = $"menu_{menu.MenuID}"
            };

            // Add event handler for menu click if it has FormName
            if (!string.IsNullOrEmpty(menu.FormName))
            {
                menuItem.Click += (sender, e) => 
                {
                    try
                    {
                        Debug.WriteLine($"Menu clicked: {menu.MenuName}");
                        Debug.WriteLine($"FormName: {menu.FormName}");
                        
                        if (sender is ToolStripMenuItem item && item.Tag is Menu clickedMenu)
                        {
                            Debug.WriteLine($"Successfully cast Tag to Menu: {clickedMenu.MenuName}");
                            FormFactory.OpenUserControl(clickedMenu.FormName, pnlMain, clickedMenu.MenuName, UpdateStatusText);
                        }
                        else
                        {
                            Debug.WriteLine("Failed to cast Tag to Menu, using direct menu reference");
                            // Use the menu variable directly from closure
                            FormFactory.OpenUserControl(menu.FormName, pnlMain, menu.MenuName, UpdateStatusText);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception in menu click: {ex.Message}");
                        MessageBox.Show($"Lỗi khi mở menu {menu.MenuName}: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
            }

            // Add sub-menus
            foreach (var subMenu in menu.SubMenus)
            {
                var subMenuItem = CreateMenuItem(subMenu);
                menuItem.DropDownItems.Add(subMenuItem);
            }

            return menuItem;
        }

        // Removed old MenuItem_Click method - now using inline lambda in CreateMenuItem
        // Removed old HandleMenuClick method - now using FormFactory directly

        // Method HandleMenuClick đã được thay thế bằng FormFactory.OpenUserControl

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

        // Show...Control methods removed - FormFactory handles all control creation dynamically

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

        #region Menu Event Handlers - Removed
        // All menu event handlers have been replaced with dynamic menu system using FormFactory
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
