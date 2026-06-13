using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QLVT.GUI.Components
{
    /// <summary>
    /// Reusable TextBox component for entering supply (vật tư) filter - configurable size
    /// </summary>
    public partial class VatTuTextBox : UserControl
    {
        private TextBox txtVatTu;

        // Events
        public event EventHandler<EventArgs> TextChanged;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyPressEventArgs> KeyPress;

        // Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The placeholder text for the textbox")]
        public string PlaceholderText
        {
            get => txtVatTu.PlaceholderText;
            set => txtVatTu.PlaceholderText = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The text content of the textbox")]
        public override string Text
        {
            get => txtVatTu.Text;
            set => txtVatTu.Text = value;
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Width of the TextBox control")]
        public int TextBoxWidth { get; set; } = 200;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Height of the TextBox control")]
        public int TextBoxHeight { get; set; } = 20;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Total width of the component")]
        public new int Width 
        { 
            get => base.Width; 
            set 
            {
                base.Width = value;
                TextBoxWidth = value;
                UpdateLayout();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Total height of the component")]
        public new int Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                TextBoxHeight = value;
                UpdateLayout();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether the textbox is read-only")]
        public bool ReadOnly
        {
            get => txtVatTu.ReadOnly;
            set => txtVatTu.ReadOnly = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font of the textbox")]
        public override Font Font
        {
            get => txtVatTu.Font;
            set => txtVatTu.Font = value;
        }

        public VatTuTextBox() : this(200, 20)
        {
        }

        public VatTuTextBox(int width, int height)
        {
            TextBoxWidth = width;
            TextBoxHeight = height;
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtVatTu = new TextBox();
            this.SuspendLayout();

            // txtVatTu - same style as BaoCaoXuatNhapTon
            this.txtVatTu.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.txtVatTu.Location = new Point(0, 0);
            this.txtVatTu.Name = "txtVatTu";
            this.txtVatTu.PlaceholderText = "Nhập mã hoặc tên vật tư...";
            this.txtVatTu.Size = new Size(TextBoxWidth, TextBoxHeight);
            this.txtVatTu.TabIndex = 1;
            this.txtVatTu.TextChanged += TxtVatTu_TextChanged;
            this.txtVatTu.KeyDown += TxtVatTu_KeyDown;
            this.txtVatTu.KeyPress += TxtVatTu_KeyPress;

            // VatTuTextBox
            this.Controls.Add(this.txtVatTu);
            this.Name = "VatTuTextBox";
            this.Size = new Size(TextBoxWidth, TextBoxHeight);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void UpdateLayout()
        {
            if (txtVatTu != null)
            {
                // Cập nhật size của TextBox
                txtVatTu.Size = new Size(TextBoxWidth, TextBoxHeight);
                
                // Cập nhật tổng size component
                this.Size = new Size(TextBoxWidth, TextBoxHeight);
            }
        }

        private void TxtVatTu_TextChanged(object? sender, EventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        private void TxtVatTu_KeyDown(object? sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        private void TxtVatTu_KeyPress(object? sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        // Additional helper methods
        public void Clear()
        {
            txtVatTu.Clear();
        }

        public void Focus()
        {
            txtVatTu.Focus();
        }

        public void SelectAll()
        {
            txtVatTu.SelectAll();
        }

        public string GetText()
        {
            return txtVatTu.Text?.Trim() ?? string.Empty;
        }

        public void SetText(string text)
        {
            txtVatTu.Text = text ?? string.Empty;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(txtVatTu.Text);
        }

        // Override to handle design time
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.DesignMode) return;
        }
    }
}