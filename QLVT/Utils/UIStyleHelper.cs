using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLVT.Utils
{
    public static class UIStyleHelper
    {
        public const int PageHeaderHeight = 50;
        public const int PageHeaderContentGap = 18;

        public static void ApplyPrimaryButtonStyle(Button button, Size? size = null)
        {
            ApplyButtonStyle(button, UIColorPalette.ButtonPrimary.Base, UIColorPalette.ButtonPrimary.Hover,
                UIColorPalette.ButtonPrimary.Text, size);
        }

        public static void ApplySuccessButtonStyle(Button button, Size? size = null)
        {
            ApplyButtonStyle(button, UIColorPalette.ButtonSuccess.Base, UIColorPalette.ButtonSuccess.Hover,
                UIColorPalette.ButtonSuccess.Text, size);
        }

        public static void ApplyWarningButtonStyle(Button button, Size? size = null)
        {
            ApplyButtonStyle(button, UIColorPalette.ButtonWarning.Base, UIColorPalette.ButtonWarning.Hover,
                UIColorPalette.ButtonWarning.Text, size);
        }

        public static void ApplyDangerButtonStyle(Button button, Size? size = null)
        {
            ApplyButtonStyle(button, UIColorPalette.ButtonDanger.Base, UIColorPalette.ButtonDanger.Hover,
                UIColorPalette.ButtonDanger.Text, size);
        }

        public static void ApplySecondaryButtonStyle(Button button, Size? size = null)
        {
            ApplyButtonStyle(button, UIColorPalette.SurfaceMuted, UIColorPalette.Border,
                UIColorPalette.TextDark, size, UIColorPalette.BorderStrong);
        }

        private static void ApplyButtonStyle(Button button, Color baseColor, Color hoverColor, Color textColor,
            Size? size = null, Color? borderColor = null)
        {
            button.Font = UIFonts.ButtonStandard;
            button.ForeColor = textColor;
            button.BackColor = baseColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = borderColor.HasValue ? 1 : 0;
            button.FlatAppearance.BorderColor = borderColor ?? baseColor;
            button.Size = size ?? new Size(128, 36);
            button.Cursor = Cursors.Hand;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseVisualStyleBackColor = false;

            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = baseColor;
        }

        public static void ApplyTitleBarStyle(Label label, DockStyle dock = DockStyle.Top, int height = PageHeaderHeight)
        {
            label.Font = UIFonts.TitleLarge;
            label.ForeColor = UIColorPalette.TextWhite;
            label.BackColor = Color.FromArgb(52, 152, 219);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Padding = Padding.Empty;
            label.Dock = dock;
            label.Height = height;
        }

        public static void ApplyStandardLabelStyle(Label label)
        {
            label.Font = UIFonts.TextStandard;
            label.ForeColor = UIColorPalette.TextBlack;
            label.AutoSize = true;
        }

        public static void ApplyStatusLabelStyle(Label label, StatusType status = StatusType.Ready)
        {
            label.Font = UIFonts.TextSmall;
            label.AutoSize = false;
            label.BackColor = UIColorPalette.BackgroundWhite;
            label.ForeColor = status switch
            {
                StatusType.Success => UIColorPalette.StatusSuccess,
                StatusType.Error => UIColorPalette.StatusError,
                StatusType.Processing => UIColorPalette.StatusProcessing,
                StatusType.Warning => UIColorPalette.StatusWarningText,
                _ => UIColorPalette.TextMuted
            };
        }

        public static void ApplyGroupBoxStyle(GroupBox groupBox, AnchorStyles? anchor = null)
        {
            groupBox.Font = UIFonts.HeaderStandard;
            groupBox.ForeColor = UIColorPalette.TextDark;
            groupBox.BackColor = UIColorPalette.BackgroundWhite;
            groupBox.FlatStyle = FlatStyle.Flat;

            if (anchor.HasValue)
            {
                groupBox.Anchor = anchor.Value;
            }
        }

        public static void ApplyPanelStyle(Panel panel)
        {
            panel.BackColor = UIColorPalette.BackgroundWhite;
            panel.Padding = new Padding(16);
        }

        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.Font = UIFonts.TextStandard;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = UIColorPalette.BackgroundWhite;
            textBox.ForeColor = UIColorPalette.TextDark;
            textBox.Height = 23;
            AttachInputBorderPainter(textBox);
        }

        public static void ApplyComboBoxStyle(ComboBox comboBox)
        {
            comboBox.Font = UIFonts.TextStandard;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.BackColor = comboBox.Enabled ? UIColorPalette.BackgroundWhite : UIColorPalette.InputDisabledBack;
            comboBox.ForeColor = comboBox.Enabled ? UIColorPalette.TextDark : UIColorPalette.TextMuted;
            comboBox.Height = 23;
            AttachInputBorderPainter(comboBox);
        }

        public static void ApplyCheckBoxStyle(CheckBox checkBox)
        {
            checkBox.Font = UIFonts.TextStandard;
            checkBox.ForeColor = UIColorPalette.TextDark;
            checkBox.BackColor = Color.Transparent;
            checkBox.AutoSize = false;
        }

        public static void ApplyDataGridViewStyle(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 34;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgv.DefaultCellStyle.Font = UIFonts.GridData;
            dgv.RowTemplate.DefaultCellStyle.Font = UIFonts.GridData;
            dgv.DefaultCellStyle.BackColor = UIColorPalette.BackgroundWhite;
            dgv.DefaultCellStyle.ForeColor = UIColorPalette.TextDark;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = UIColorPalette.TextDark;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = UIColorPalette.SurfaceMuted;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = UIColorPalette.SurfaceMuted;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = UIColorPalette.TextDark;
            dgv.ColumnHeadersDefaultCellStyle.Font = UIFonts.GridHeader;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = UIColorPalette.SurfaceMuted;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.BackgroundColor = UIColorPalette.BackgroundWhite;
            dgv.GridColor = UIColorPalette.Border;
        }

        public static DataGridViewTextBoxColumn CreateReadOnlyColumn(
            string name,
            string headerText,
            string? dataPropertyName = null,
            int width = 100,
            int fillWeight = 10)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = dataPropertyName ?? name,
                Width = width,
                FillWeight = fillWeight,
                ReadOnly = true
            };
        }

        public static DataGridViewTextBoxColumn CreateNumericColumn(
            string name,
            string headerText,
            string? dataPropertyName = null,
            int width = 100,
            int fillWeight = 10,
            bool readOnly = true,
            string format = "N2")
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = dataPropertyName ?? name,
                Width = width,
                FillWeight = fillWeight,
                ReadOnly = readOnly,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = format,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };
        }

        public static void ApplyFormStyle(Control control)
        {
            control.BackColor = UIColorPalette.BackgroundLight;
            control.Font = UIFonts.TextStandard;
        }

        public static void ApplyControlTreeStyle(Control root)
        {
            ApplyFormStyle(root);
            ApplyControlTreeStyleRecursive(root, root);
        }

        private static void ApplyControlTreeStyleRecursive(Control control, Control root)
        {
            foreach (Control child in control.Controls)
            {
                switch (child)
                {
                    case Label label when label.Name.Equals("lblTitle", StringComparison.OrdinalIgnoreCase)
                                      && label.Dock == DockStyle.Top:
                        ApplyTitleBarStyle(label);
                        break;
                    case Label label:
                        ApplyStandardLabelStyle(label);
                        break;
                    case GroupBox groupBox:
                        ApplyGroupBoxStyle(groupBox);
                        break;
                    case TextBox textBox:
                        ApplyTextBoxStyle(textBox);
                        break;
                    case ComboBox comboBox:
                        ApplyComboBoxStyle(comboBox);
                        break;
                    case CheckBox checkBox:
                        ApplyCheckBoxStyle(checkBox);
                        break;
                    case DataGridView dataGridView:
                        var autoGenerateColumns = dataGridView.AutoGenerateColumns;
                        ApplyDataGridViewStyle(dataGridView);
                        dataGridView.AutoGenerateColumns = autoGenerateColumns;
                        break;
                    case Button button:
                        ApplyButtonStyleByText(button);
                        break;
                    case DateTimePicker dateTimePicker:
                        dateTimePicker.Font = UIFonts.TextStandard;
                        break;
                    case NumericUpDown numericUpDown:
                        numericUpDown.Font = UIFonts.TextStandard;
                        numericUpDown.BackColor = BackgroundColorForInput(numericUpDown.Enabled);
                        numericUpDown.ForeColor = UIColorPalette.TextDark;
                        numericUpDown.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case Panel panel:
                        panel.BackColor = panel == root ? UIColorPalette.BackgroundLight : panel.BackColor;
                        break;
                }

                if (child.HasChildren)
                {
                    ApplyControlTreeStyleRecursive(child, root);
                }
            }
        }

        private static void ApplyButtonStyleByText(Button button)
        {
            var text = button.Text.ToLowerInvariant();

            if (text.Contains("xóa") || text.Contains("xoá") || text.Contains("hủy") || text.Contains("huy")
                || text.Contains("delete") || text.Contains("cancel"))
            {
                ApplyDangerButtonStyle(button, button.Size);
                return;
            }

            if (text.Contains("làm mới") || text.Contains("refresh") || text.Contains("xuất") || text.Contains("export")
                || text.Contains("lưu") || text.Contains("save"))
            {
                ApplySuccessButtonStyle(button, button.Size);
                return;
            }

            if (text.Contains("xác nhận") || text.Contains("bắt đầu") || text.Contains("hoàn ứng")
                || text.Contains("confirm") || text.Contains("start"))
            {
                ApplyWarningButtonStyle(button, button.Size);
                return;
            }

            ApplyPrimaryButtonStyle(button, button.Size);
        }

        private static Color BackgroundColorForInput(bool enabled)
        {
            return enabled ? UIColorPalette.BackgroundWhite : UIColorPalette.InputDisabledBack;
        }

        private static void AttachInputBorderPainter(Control control)
        {
            void AttachToParent()
            {
                if (control.Parent == null) return;

                var parent = control.Parent;
                PaintEventHandler paintHandler = (_, e) =>
                {
                    if (control.IsDisposed || !control.Visible) return;

                    var borderColor = !control.Enabled
                        ? UIColorPalette.InputDisabledBorder
                        : control.Focused
                            ? UIColorPalette.InputFocusBorder
                            : UIColorPalette.InputBorder;

                    var rect = new Rectangle(control.Left - 1, control.Top - 1, control.Width + 1, control.Height + 1);
                    using var pen = new Pen(borderColor, 1);
                    e.Graphics.DrawRectangle(pen, rect);
                };

                parent.Paint += paintHandler;
                control.Enter += (_, _) => parent.Invalidate(GetInputBorderInvalidateRect(control));
                control.Leave += (_, _) => parent.Invalidate(GetInputBorderInvalidateRect(control));
                control.EnabledChanged += (_, _) =>
                {
                    control.BackColor = control.Enabled ? UIColorPalette.BackgroundWhite : UIColorPalette.InputDisabledBack;
                    parent.Invalidate(GetInputBorderInvalidateRect(control));
                };
                control.LocationChanged += (_, _) => parent.Invalidate();
                control.SizeChanged += (_, _) => parent.Invalidate();
                parent.Invalidate(GetInputBorderInvalidateRect(control));
            }

            if (control.Parent == null)
            {
                control.ParentChanged += (_, _) => AttachToParent();
            }
            else
            {
                AttachToParent();
            }
        }

        private static Rectangle GetInputBorderInvalidateRect(Control control)
        {
            return new Rectangle(control.Left - 2, control.Top - 2, control.Width + 4, control.Height + 4);
        }

        public static void SetStatusMessage(Label label, string message, StatusType status)
        {
            label.Text = message;
            ApplyStatusLabelStyle(label, status);
        }
    }

    public enum StatusType
    {
        Success,
        Error,
        Processing,
        Ready,
        Warning
    }
}
