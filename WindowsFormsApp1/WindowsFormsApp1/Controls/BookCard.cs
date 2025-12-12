using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Controls
{
    public partial class BookCard : UserControl
    {
        private Book _book;

        public event EventHandler BookClicked;
        public event EventHandler MenuClicked;
        public event EventHandler RestoreClicked;
        public event EventHandler PermanentDeleteClicked;

        private PictureBox coverBox;
        private Label titleLabel;
        private Label progressLabel;
        private Button menuButton;
        private Button restoreButton;
        private Button deleteButton;
        private Panel infoPanel;
        private Panel trashButtonsPanel;

        // Màu sắc
        private readonly Color clrRestoreNormal = Color.FromArgb(46, 125, 50);
        private readonly Color clrRestoreHover = Color.FromArgb(67, 160, 71);
        private readonly Color clrDeleteNormal = Color.FromArgb(198, 40, 40);
        private readonly Color clrDeleteHover = Color.FromArgb(229, 57, 53);

        public BookCard()
        {
            InitializeCustomComponents();

            this.Click += (s, e) => BookClicked?.Invoke(this, e);
            this.Cursor = Cursors.Hand;
            this.BackColor = Color.Transparent;
            this.Size = new Size(150, 240);
            this.Margin = new Padding(15);
        }

        public Book Book
        {
            get { return _book; }
            set
            {
                _book = value;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (_book == null) return;

            // 1. Hiển thị Title
            titleLabel.Text = _book.Title;

            // 2. Hiển thị Tiến độ
            if (_book.TotalPages > 0)
            {
                progressLabel.Text = $"{_book.Progress:0.00}%";
            }
            else
            {
                progressLabel.Text = "0.00%";
            }

            // 3. Hiển thị nút thùng rác nếu sách đã bị xóa
            bool isDeleted = _book.IsDeleted;
            trashButtonsPanel.Visible = isDeleted;
            progressLabel.Visible = !isDeleted;
            menuButton.Visible = !isDeleted;

            // 4. Xử lý hiển thị Ảnh bìa
            string imagePath = _book.CoverImagePath;
            bool imageLoaded = false;

            if (!string.IsNullOrEmpty(imagePath))
            {
                if (File.Exists(imagePath))
                {
                    LoadImageSafe(imagePath);
                    imageLoaded = true;
                }
                else
                {
                    try
                    {
                        string fileName = Path.GetFileName(imagePath);
                        string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CoverImages", fileName);

                        if (File.Exists(localPath))
                        {
                            LoadImageSafe(localPath);
                            imageLoaded = true;
                        }
                    }
                    catch { }
                }
            }

            if (!imageLoaded)
            {
                coverBox.Image = null;
                coverBox.BackColor = Color.FromArgb(60, 60, 60);
            }
        }

        private void LoadImageSafe(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    if (coverBox.Image != null) coverBox.Image.Dispose();
                    coverBox.Image = Image.FromStream(fs);
                }
            }
            catch
            {
                coverBox.Image = null;
            }
        }

        private void InitializeCustomComponents()
        {
            // 1. Cover Image
            coverBox = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 190,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(60, 60, 60)
            };
            coverBox.Click += (s, e) => BookClicked?.Invoke(this, e);

            // 2. Info Panel
            infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0)
            };

            // 3. Title
            titleLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                AutoEllipsis = true,
                Text = "Book Title"
            };
            titleLabel.Click += (s, e) => BookClicked?.Invoke(this, e);

            // 4. Bottom Row (cho sách bình thường)
            Panel bottomRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 20
            };

            progressLabel = new Label
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                Text = "0.00%",
                TextAlign = ContentAlignment.MiddleLeft
            };

            menuButton = new Button
            {
                Text = "...",
                Dock = DockStyle.Right,
                Width = 30,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight,
                Cursor = Cursors.Hand
            };
            menuButton.FlatAppearance.BorderSize = 0;
            menuButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            menuButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            menuButton.Click += (s, e) => MenuClicked?.Invoke(this, e);

            // 5. Panel nút cho thùng rác - Thiết kế lại đẹp hơn
            trashButtonsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                Visible = false,
                Padding = new Padding(2, 4, 2, 2)
            };

            // Nút Khôi phục - Thiết kế mới
            restoreButton = new Button
            {
                Text = "↩ Khôi phục",
                Size = new Size(70, 28),
                Location = new Point(2, 4),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = clrRestoreNormal,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            restoreButton.FlatAppearance.BorderSize = 1;
            restoreButton.FlatAppearance.BorderColor = Color.FromArgb(76, 175, 80);
            restoreButton.FlatAppearance.MouseOverBackColor = clrRestoreHover;
            restoreButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(27, 94, 32);
            restoreButton.Click += (s, e) => RestoreClicked?.Invoke(this, e);
            
            // Tooltip cho nút khôi phục
            ToolTip tipRestore = new ToolTip();
            tipRestore.SetToolTip(restoreButton, "Khôi phục sách về thư viện");

            // Nút Xóa vĩnh viễn - Thiết kế mới
            deleteButton = new Button
            {
                Text = "✕ Xóa",
                Size = new Size(70, 28),
                Location = new Point(76, 4),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = clrDeleteNormal,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            deleteButton.FlatAppearance.BorderSize = 1;
            deleteButton.FlatAppearance.BorderColor = Color.FromArgb(239, 83, 80);
            deleteButton.FlatAppearance.MouseOverBackColor = clrDeleteHover;
            deleteButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(183, 28, 28);
            deleteButton.Click += (s, e) => PermanentDeleteClicked?.Invoke(this, e);
            
            // Tooltip cho nút xóa
            ToolTip tipDelete = new ToolTip();
            tipDelete.SetToolTip(deleteButton, "Xóa vĩnh viễn sách này");

            trashButtonsPanel.Controls.Add(deleteButton);
            trashButtonsPanel.Controls.Add(restoreButton);

            bottomRow.Controls.Add(menuButton);
            bottomRow.Controls.Add(progressLabel);

            infoPanel.Controls.Add(trashButtonsPanel);
            infoPanel.Controls.Add(bottomRow);
            infoPanel.Controls.Add(titleLabel);

            this.Controls.Add(infoPanel);
            this.Controls.Add(coverBox);
        }
    }
}