using System;
using System.Reflection;
using System.Windows.Forms;
using QLVT.GUI;

namespace QLVT.Utils
{
    /// <summary>
    /// Factory class để tạo và mở forms/controls động theo FormName
    /// </summary>
    public static class FormFactory
    {
        /// <summary>
        /// Mở UserControl trong main panel
        /// </summary>
        /// <param name="formName">Tên class của UserControl/Form</param>
        /// <param name="mainPanel">Panel chứa control</param>
        /// <param name="title">Tiêu đề hiển thị</param>
        /// <returns>Thành công hay không</returns>
        public static bool OpenUserControl(string formName, Panel mainPanel, string title, Action<string> updateStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(formName))
                {
                    updateStatus("FormName trống");
                    return false;
                }

                updateStatus($"Đang mở: {title}...");

                // Debug: Log FormName being searched
                System.Diagnostics.Debug.WriteLine($"FormFactory: Searching for {formName}");

                // Tìm UserControl theo tên
                var userControl = CreateUserControl(formName);
                if (userControl != null)
                {
                    System.Diagnostics.Debug.WriteLine($"FormFactory: Found UserControl {formName}");
                    LoadUserControlToPanel(userControl, mainPanel, title, updateStatus);
                    return true;
                }

                // Nếu không tìm thấy UserControl, thử tìm Form
                System.Diagnostics.Debug.WriteLine($"FormFactory: UserControl not found, searching for Form {formName}");
                var form = CreateForm(formName);
                if (form != null)
                {
                    System.Diagnostics.Debug.WriteLine($"FormFactory: Found Form {formName}");
                    form.ShowDialog();
                    updateStatus($"Đã mở form: {title}");
                    return true;
                }

                System.Diagnostics.Debug.WriteLine($"FormFactory: Neither UserControl nor Form found for {formName}");
                updateStatus($"Không tìm thấy form/control: {formName}");
                MessageBox.Show($"Không tìm thấy form/control: {formName}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (Exception ex)
            {
                updateStatus($"Lỗi khi mở {title}");
                MessageBox.Show($"Lỗi khi mở {title}: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Tạo UserControl từ FormName
        /// </summary>
        /// <param name="formName">Tên class UserControl</param>
        /// <returns>UserControl instance hoặc null</returns>
        private static UserControl? CreateUserControl(string formName)
        {
            try
            {
                // Danh sách namespace để tìm kiếm
                string[] namespaces = { 
                    "QLVT.GUI", 
                    "QLVT.ERP.GUI", 
                    "QLVT" 
                };

                var assembly = Assembly.GetExecutingAssembly();

                foreach (var ns in namespaces)
                {
                    var fullTypeName = $"{ns}.{formName}";
                    var type = assembly.GetType(fullTypeName);
                    
                    if (type != null && type.IsSubclassOf(typeof(UserControl)))
                    {
                        return (UserControl?)Activator.CreateInstance(type);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tạo UserControl {formName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tạo Form từ FormName
        /// </summary>
        /// <param name="formName">Tên class Form</param>
        /// <returns>Form instance hoặc null</returns>
        private static Form? CreateForm(string formName)
        {
            try
            {
                // Danh sách namespace để tìm kiếm
                string[] namespaces = { 
                    "QLVT.GUI", 
                    "QLVT.ERP.GUI", 
                    "QLVT" 
                };

                var assembly = Assembly.GetExecutingAssembly();

                foreach (var ns in namespaces)
                {
                    var fullTypeName = $"{ns}.{formName}";
                    System.Diagnostics.Debug.WriteLine($"FormFactory: Trying to find type {fullTypeName}");
                    var type = assembly.GetType(fullTypeName);
                    
                    if (type != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"FormFactory: Found type {fullTypeName}, checking if it's a Form");
                        if (type.IsSubclassOf(typeof(Form)))
                        {
                            System.Diagnostics.Debug.WriteLine($"FormFactory: Creating instance of {fullTypeName}");
                            return (Form?)Activator.CreateInstance(type);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"FormFactory: Type {fullTypeName} is not a Form");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"FormFactory: Type {fullTypeName} not found");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tạo Form {formName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Load UserControl vào Panel
        /// </summary>
        /// <param name="userControl">UserControl cần load</param>
        /// <param name="mainPanel">Panel chứa</param>
        /// <param name="title">Tiêu đề</param>
        /// <param name="updateStatus">Action cập nhật status</param>
        private static void LoadUserControlToPanel(UserControl userControl, Panel mainPanel, 
            string title, Action<string> updateStatus)
        {
            try
            {
                // Clear existing controls
                mainPanel.Controls.Clear();
                
                // Configure UserControl
                userControl.Dock = DockStyle.Fill;
                
                // Add to main panel
                mainPanel.Controls.Add(userControl);
                
                // Update status
                updateStatus($"Đã mở: {title}");
            }
            catch (Exception ex)
            {
                updateStatus($"Lỗi khi load {title}");
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả UserControl có sẵn
        /// </summary>
        /// <returns>Danh sách tên UserControl</returns>
        public static List<string> GetAvailableUserControls()
        {
            var userControls = new List<string>();
            
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(UserControl)) && 
                        !type.IsAbstract && 
                        type.IsPublic)
                    {
                        userControls.Add(type.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lấy danh sách UserControl: {ex.Message}");
            }

            return userControls.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Lấy danh sách tất cả Form có sẵn
        /// </summary>
        /// <returns>Danh sách tên Form</returns>
        public static List<string> GetAvailableForms()
        {
            var forms = new List<string>();
            
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(Form)) && 
                        !type.IsAbstract && 
                        type.IsPublic &&
                        type.Name != "MainForm" && 
                        type.Name != "LoginForm")
                    {
                        forms.Add(type.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lấy danh sách Form: {ex.Message}");
            }

            return forms.OrderBy(x => x).ToList();
        }
    }
}