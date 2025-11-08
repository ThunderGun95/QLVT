using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLVT.Utils
{
    /// <summary>
    /// Helper class to apply modern UI styling to WinForms controls
    /// Ensures consistency across all forms in the application
    /// </summary>
    public static class UIStyleHelper
    {
        // ===== BUTTON STYLING =====
        
        /// <summary>
        /// Apply Primary (Blue) button style with hover effects
        /// Use for: Search, Query, Load actions
        /// </summary>
        public static void ApplyPrimaryButtonStyle(Button button, Size? size = null)
        {
            button.Font = UIFonts.ButtonStandard;
            button.ForeColor = UIColorPalette.ButtonPrimary.Text;
            button.BackColor = UIColorPalette.ButtonPrimary.Base;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Size = size ?? new Size(120, 35);
            button.Cursor = Cursors.Hand;
            
            // Add hover effects
            button.MouseEnter += (s, e) => button.BackColor = UIColorPalette.ButtonPrimary.Hover;
            button.MouseLeave += (s, e) => button.BackColor = UIColorPalette.ButtonPrimary.Base;
        }
        
        /// <summary>
        /// Apply Success (Green) button style with hover effects
        /// Use for: Refresh, Reload, Save actions
        /// </summary>
        public static void ApplySuccessButtonStyle(Button button, Size? size = null)
        {
            button.Font = UIFonts.ButtonStandard;
            button.ForeColor = UIColorPalette.ButtonSuccess.Text;
            button.BackColor = UIColorPalette.ButtonSuccess.Base;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Size = size ?? new Size(120, 35);
            button.Cursor = Cursors.Hand;
            
            // Add hover effects
            button.MouseEnter += (s, e) => button.BackColor = UIColorPalette.ButtonSuccess.Hover;
            button.MouseLeave += (s, e) => button.BackColor = UIColorPalette.ButtonSuccess.Base;
        }
        
        /// <summary>
        /// Apply Warning (Orange) button style with hover effects
        /// Use for: Confirm, Submit, Execute actions
        /// </summary>
        public static void ApplyWarningButtonStyle(Button button, Size? size = null)
        {
            button.Font = UIFonts.ButtonStandard;
            button.ForeColor = UIColorPalette.ButtonWarning.Text;
            button.BackColor = UIColorPalette.ButtonWarning.Base;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Size = size ?? new Size(120, 35);
            button.Cursor = Cursors.Hand;
            
            // Add hover effects
            button.MouseEnter += (s, e) => button.BackColor = UIColorPalette.ButtonWarning.Hover;
            button.MouseLeave += (s, e) => button.BackColor = UIColorPalette.ButtonWarning.Base;
        }
        
        /// <summary>
        /// Apply Danger (Red) button style with hover effects
        /// Use for: Delete, Cancel, Remove actions
        /// </summary>
        public static void ApplyDangerButtonStyle(Button button, Size? size = null)
        {
            button.Font = UIFonts.ButtonStandard;
            button.ForeColor = UIColorPalette.ButtonDanger.Text;
            button.BackColor = UIColorPalette.ButtonDanger.Base;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Size = size ?? new Size(120, 35);
            button.Cursor = Cursors.Hand;
            
            // Add hover effects
            button.MouseEnter += (s, e) => button.BackColor = UIColorPalette.ButtonDanger.Hover;
            button.MouseLeave += (s, e) => button.BackColor = UIColorPalette.ButtonDanger.Base;
        }
        
        
        // ===== LABEL STYLING =====
        
        /// <summary>
        /// Apply title bar label style (large, bold, white on blue)
        /// </summary>
        public static void ApplyTitleBarStyle(Label label, DockStyle dock = DockStyle.Top, int height = 50)
        {
            label.Font = UIFonts.TitleLarge;
            label.ForeColor = UIColorPalette.TextWhite;
            label.BackColor = UIColorPalette.PrimaryBlueDark;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = dock;
            label.Height = height;
        }
        
        /// <summary>
        /// Apply standard label style
        /// </summary>
        public static void ApplyStandardLabelStyle(Label label)
        {
            label.Font = UIFonts.TextStandard;
            label.ForeColor = UIColorPalette.TextDark;
            label.AutoSize = true;
        }
        
        /// <summary>
        /// Apply status label style (with icon support)
        /// </summary>
        public static void ApplyStatusLabelStyle(Label label, StatusType status = StatusType.Ready)
        {
            label.Font = UIFonts.TextStandard;
            label.AutoSize = true;
            
            switch (status)
            {
                case StatusType.Success:
                    label.ForeColor = UIColorPalette.StatusSuccess;
                    break;
                case StatusType.Error:
                    label.ForeColor = UIColorPalette.StatusError;
                    break;
                case StatusType.Processing:
                    label.ForeColor = UIColorPalette.StatusProcessing;
                    break;
                case StatusType.Ready:
                default:
                    label.ForeColor = UIColorPalette.StatusSuccess;
                    break;
            }
        }
        
        
        // ===== GROUPBOX STYLING =====
        
        /// <summary>
        /// Apply modern flat GroupBox style
        /// </summary>
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
        
        
        // ===== TEXTBOX STYLING =====
        
        /// <summary>
        /// Apply standard TextBox style
        /// </summary>
        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.Font = UIFonts.TextStandard;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }
        
        
        // ===== DATAGRIDVIEW STYLING =====
        
        /// <summary>
        /// Apply modern DataGridView style with responsive columns
        /// </summary>
        public static void ApplyDataGridViewStyle(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Font style - Regular (không in đậm)
            dgv.DefaultCellStyle.Font = UIFonts.GridData;
            dgv.RowTemplate.DefaultCellStyle.Font = UIFonts.GridData;
            
            // Background
            dgv.BackgroundColor = UIColorPalette.BackgroundWhite;
            dgv.GridColor = Color.LightGray;
        }
        
        /// <summary>
        /// Create a standard read-only text column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateReadOnlyColumn(
            string name, 
            string headerText, 
            string dataPropertyName = null,
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
        
        /// <summary>
        /// Create a numeric column with right alignment
        /// </summary>
        public static DataGridViewTextBoxColumn CreateNumericColumn(
            string name,
            string headerText,
            string dataPropertyName = null,
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
        
        
        // ===== FORM STYLING =====
        
        /// <summary>
        /// Apply standard form/UserControl background
        /// </summary>
        public static void ApplyFormStyle(Control control)
        {
            control.BackColor = UIColorPalette.BackgroundLight;
        }
        
        
        // ===== STATUS MESSAGE HELPERS =====
        
        /// <summary>
        /// Set status message with appropriate icon and color
        /// </summary>
        public static void SetStatusMessage(Label label, string message, StatusType status)
        {
            string icon = status switch
            {
                StatusType.Success => "✅",
                StatusType.Error => "❌",
                StatusType.Processing => "🔄",
                StatusType.Ready => "",
                _ => ""
            };
            
            label.Text = string.IsNullOrEmpty(icon) ? message : $"{icon} {message}";
            ApplyStatusLabelStyle(label, status);
        }
    }
    
    
    /// <summary>
    /// Status types for status messages
    /// </summary>
    public enum StatusType
    {
        Success,
        Error,
        Processing,
        Ready
    }
}
