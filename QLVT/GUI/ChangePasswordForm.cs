using QLVT.BLL;

namespace QLVT.GUI
{
    public partial class ChangePasswordForm : Form
    {
        private readonly AuthenticationBLL authBLL;

        public ChangePasswordForm()
        {
            InitializeComponent();
            authBLL = new AuthenticationBLL();
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            txtOldPassword.Focus();
            lblError.Text = string.Empty;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            PerformChangePassword();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            ValidatePasswords();
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            ValidatePasswords();
        }

        private void ValidatePasswords()
        {
            lblError.Text = string.Empty;

            if (txtNewPassword.Text.Length > 0 && txtNewPassword.Text.Length < 6)
            {
                ShowError("Mật khẩu mới phải có ít nhất 6 ký tự!");
                return;
            }

            if (txtConfirmPassword.Text.Length > 0 && txtNewPassword.Text != txtConfirmPassword.Text)
            {
                ShowError("Mật khẩu xác nhận không khớp!");
                return;
            }

            if (txtNewPassword.Text.Length >= 6 && txtNewPassword.Text == txtConfirmPassword.Text)
            {
                lblError.Text = "✓ Mật khẩu hợp lệ";
                lblError.ForeColor = Color.Green;
            }
        }

        private void PerformChangePassword()
        {
            try
            {
                lblError.Text = string.Empty;

                // Get input values
                string oldPassword = txtOldPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                // Validate input
                if (string.IsNullOrEmpty(oldPassword))
                {
                    ShowError("Vui lòng nhập mật khẩu cũ!");
                    txtOldPassword.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    ShowError("Vui lòng nhập mật khẩu mới!");
                    txtNewPassword.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(confirmPassword))
                {
                    ShowError("Vui lòng xác nhận mật khẩu mới!");
                    txtConfirmPassword.Focus();
                    return;
                }

                if (newPassword.Length < 6)
                {
                    ShowError("Mật khẩu mới phải có ít nhất 6 ký tự!");
                    txtNewPassword.Focus();
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    ShowError("Mật khẩu xác nhận không khớp!");
                    txtConfirmPassword.Focus();
                    return;
                }

                if (oldPassword == newPassword)
                {
                    ShowError("Mật khẩu mới phải khác mật khẩu cũ!");
                    txtNewPassword.Focus();
                    return;
                }

                // Disable buttons during processing
                btnChange.Enabled = false;
                btnChange.Text = "Đang xử lý...";

                // Perform password change
                var result = authBLL.ChangePassword(oldPassword, newPassword);

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    ShowError(result.Message);
                    txtOldPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi không xác định: {ex.Message}");
            }
            finally
            {
                // Re-enable buttons
                btnChange.Enabled = true;
                btnChange.Text = "Đổi";
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.ForeColor = Color.Red;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                PerformChangePassword();
                return true;
            }
            else if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            
            return base.ProcessDialogKey(keyData);
        }
    }
}
