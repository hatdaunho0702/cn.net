using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public class EditBookForm : Form
    {
        public bool IsUpdated { get; private set; } = false;
        private Book _book;
        private TextBox txtTitle, txtAuthor, txtDesc;

        public EditBookForm(Book book)
        {
            _book = book;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Sửa thông tin sách";
            this.Size = new Size(450, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(45, 45, 48); // Dark Theme
            this.Padding = new Padding(2); // Border

            // 1. Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(60, 60, 63) };
            Label lblHeader = new Label
            {
                Text = "Sửa thông tin sách",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };
            Button btnClose = new Button { Text = "✕", Dock = DockStyle.Right, Width = 40, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, Cursor = Cursors.Hand };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            pnlHeader.Controls.Add(lblHeader);
            pnlHeader.Controls.Add(btnClose);
            this.Controls.Add(pnlHeader);

            // Drag Window Logic
            pnlHeader.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); } };

            // 2. Content
            int startY = 60;
            int gap = 70;

            // Title
            AddLabel("TIÊU ĐỀ SÁCH", 30, startY);
            txtTitle = AddTextBox(30, startY + 25, 1);

            // Author
            AddLabel("TÁC GIẢ (ngăn cách bằng dấu phẩy)", 30, startY + gap);
            txtAuthor = AddTextBox(30, startY + gap + 25, 2);

            // Description
            AddLabel("MÔ TẢ NGẮN", 30, startY + gap * 2);
            txtDesc = AddTextBox(30, startY + gap * 2 + 25, 3, true);

            // 3. Buttons
            Button btnSave = new Button
            {
                Text = "LƯU THAY ĐỔI",
                Location = new Point(240, 350),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            Button btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(140, 350),
                Size = new Size(90, 40),
                BackColor = Color.Transparent,
                ForeColor = Color.Silver,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.BorderColor = Color.Gray;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            // Draw Border
            this.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.DimGray, ButtonBorderStyle.Solid);
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label { Text = text, Location = new Point(x, y), AutoSize = true, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9) });
        }

        private TextBox AddTextBox(int x, int y, int tabIndex, bool multiline = false)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(370, multiline ? 80 : 30),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11),
                TabIndex = tabIndex,
                Multiline = multiline
            };
            this.Controls.Add(txt);
            return txt;
        }

        private void LoadData()
        {
            if (_book == null) return;
            txtTitle.Text = _book.Title;
            txtAuthor.Text = _book.Author;
            txtDesc.Text = _book.Description;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Tiêu đề không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataManager.Instance.UpdateBookInfo(_book.Id, txtTitle.Text.Trim(), txtAuthor.Text.Trim(), txtDesc.Text.Trim());
                IsUpdated = true;
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool ReleaseCapture();
    }
}