using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLVT.Models;

namespace QLVT.GUI
{
    public partial class WarehouseSelectionForm : Form
    {
        public int SelectedWarehouseId { get; private set; }
        private List<Warehouse> warehouses;
        private ComboBox cboWarehouses;
        private Button btnOK;
        private Button btnCancel;
        private Label lblInstruction;

        public WarehouseSelectionForm(List<Warehouse> warehouseList)
        {
            warehouses = warehouseList;
            InitializeComponent();
            LoadWarehouses();
        }

        private void InitializeComponent()
        {
            this.Text = "Chọn kho nhân viên đích";
            this.Size = new Size(400, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblInstruction = new Label
            {
                Text = "Chọn kho nhân viên để chuyển vật tư:",
                Location = new Point(20, 20),
                Size = new Size(340, 20),
                Font = new Font("Microsoft Sans Serif", 9F)
            };
            this.Controls.Add(lblInstruction);

            cboWarehouses = new ComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(340, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 9F),
                DisplayMember = "TenKho",
                ValueMember = "Id"
            };
            this.Controls.Add(cboWarehouses);

            btnOK = new Button
            {
                Text = "✅ Chọn",
                Location = new Point(200, 100),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            btnCancel = new Button
            {
                Text = "❌ Hủy",
                Location = new Point(290, 100),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadWarehouses()
        {
            if (warehouses != null && warehouses.Any())
            {
                cboWarehouses.DataSource = warehouses;
                cboWarehouses.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Không có kho nhân viên nào trong hệ thống!", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (cboWarehouses.SelectedValue != null)
            {
                SelectedWarehouseId = (int)cboWarehouses.SelectedValue;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn kho!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
