using QLVT.BLL;
using QLVT.Utils;

namespace QLVT.GUI
{
    public partial class LoginForm : Form
    {
        private readonly AuthenticationBLL authBLL;

        public LoginForm()
        {
            InitializeComponent();
            QLVT.Utils.UIStyleHelper.ApplyControlTreeStyle(this);
            authBLL = new AuthenticationBLL();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Set focus to username textbox
            txtUsername.Focus();
            
            // Clear any previous error messages
            lblError.Text = string.Empty;

            // Test database connection
            if (!DatabaseHelper.TestConnection())
            {
                ShowError("Không thể kết nối đến database. Vui lòng kiểm tra lại cấu hình!");
                btnLogin.Enabled = false;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            PerformLogin();
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformLogin();
            }
        }

        private void PerformLogin()
        {
            try
            {
                // Clear previous error
                lblError.Text = string.Empty;

                // Get input values
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Validate input
                if (string.IsNullOrEmpty(username))
                {
                    ShowError("Vui lòng nhập tên đăng nhập!");
                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    ShowError("Vui lòng nhập mật khẩu!");
                    txtPassword.Focus();
                    return;
                }

                // Disable login button during authentication
                btnLogin.Enabled = false;
                btnLogin.Text = "Đang xử lý...";

                // Perform login
                var result = authBLL.Login(username, password);

                if (result.Success)
                {
                    // Login successful - chuyển thẳng vào MainForm
                    this.Hide();
                    
                    var mainForm = new MainForm();
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    // Login failed
                    ShowError(result.Message);
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi không xác định: {ex.Message}");
            }
            finally
            {
                // Re-enable login button
                btnLogin.Enabled = true;
                btnLogin.Text = "Đăng nhập";
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.ForeColor = Color.Red;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Exit application when login form is closed
            Application.Exit();
            base.OnFormClosing(e);
        }
    }
}
