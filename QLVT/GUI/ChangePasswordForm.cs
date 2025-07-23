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
            // Set focus to old password textbox
            txtOldPassword.Focus();
            
            // Clear error message
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
            ValidatePasswordMatch();
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            ValidatePasswordMatch();
        }

        private void ValidatePasswordMatch()
        {
            if (!string.IsNullOrEmpty(txtNewPassword.Text) && !string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    ShowError("Mật khẩu xác nhận không khớp!");
                    btnChange.Enabled = false;
                }
                else
                {
                    lblError.Text = string.Empty;
                    btnChange.Enabled = true;
                }
            }
        }

        private void PerformChangePassword()
        {
            try
            {
                // Clear previous error
                lblError.Text = string.Empty;

                // Get input values
                string oldPassword = txtOldPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                // Validate input
                if (string.IsNullOrWhiteSpace(oldPassword))
                {
                    ShowError("Vui lòng nhập mật khẩu cũ!");
                    txtOldPassword.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ShowError("Vui lòng nhập mật khẩu mới!");
                    txtNewPassword.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ShowError("Vui lòng xác nhận mật khẩu!");
                    txtConfirmPassword.Focus();
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    ShowError("Mật khẩu xác nhận không khớp!");
                    txtConfirmPassword.Focus();
                    return;
                }

                if (newPassword.Length < 6)
                {
                    ShowError("Mật khẩu mới phải có ít nhất 6 ký tự!");
                    txtNewPassword.Focus();
                    return;
                }

                if (oldPassword == newPassword)
                {
                    ShowError("Mật khẩu mới phải khác mật khẩu cũ!");
                    txtNewPassword.Focus();
                    return;
                }

                // Disable button during processing
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
                    txtOldPassword.Clear();
                    txtOldPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi không xác định: {ex.Message}");
            }
            finally
            {
                // Re-enable button
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
                if (txtOldPassword.Focused)
                {
                    txtNewPassword.Focus();
                    return true;
                }
                else if (txtNewPassword.Focused)
                {
                    txtConfirmPassword.Focus();
                    return true;
                }
                else if (txtConfirmPassword.Focused)
                {
                    PerformChangePassword();
                    return true;
                }
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
