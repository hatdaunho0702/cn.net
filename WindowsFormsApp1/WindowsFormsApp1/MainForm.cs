using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using WindowsFormsApp1.Controls;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        #region Theme Colors
        private readonly Color clrBackground = Color.FromArgb(32, 33, 36);
        private readonly Color clrSidebar = Color.FromArgb(25, 25, 27);
        private readonly Color clrTopBar = Color.FromArgb(40, 41, 45);
        private readonly Color clrAccent = Color.FromArgb(138, 180, 248);
        private readonly Color clrTextActive = Color.White;
        private readonly Color clrTextInactive = Color.FromArgb(154, 160, 166);
        private readonly Color clrHover = Color.FromArgb(60, 64, 67);
        private readonly Color clrCardBg = Color.FromArgb(45, 45, 48); // Màu nền thẻ Note
        #endregion

        #region UI Components
        // Layout Containers
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Panel topBar;
        private FlowLayoutPanel booksPanel;

        // Search & Filter
        private Panel searchPanel;
        private TextBox searchBox;
        private Label searchIcon;
        private Panel pnlFilterBar;
        private ComboBox cmbFilterBook;
        private System.Windows.Forms.Timer searchDebounceTimer;

        // Sidebar Controls
        private Button menuButton;
        private Button btnShelfToggle;
        private FlowLayoutPanel pnlShelfContainer;
        private Dictionary<string, Button> sidebarButtons = new Dictionary<string, Button>();

        // TopBar Controls
        private ModernButton btnAddBook; // Nút mới gộp chức năng
        private ModernButton sortButton;
        private ModernButton btnReport; // Nút báo cáo sách
        private Button userButton;
        private Label lblUsername;
        private Label logoLabel;

        // Menus
        private ContextMenuStrip authMenu;
        private ContextMenuStrip importMenu;
        private ContextMenuStrip sortMenu;   // Menu sắp xếp

        // Footer / Status
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatusTotal;
        private ToolStripStatusLabel lblStatusState;
        #endregion

        #region State Variables
        private string currentView = "Books";
        private string currentSortBy = "Reading progress";
        private bool sortAscending = false;
        private bool isSidebarExpanded = true;
        private bool isShelfExpanded = true;
        private int activeShelfId = -1;
        private User _currentUser = null;
        #endregion

        public MainForm()
        {
            this.Text = "Koodo Reader";
            this.Size = new Size(1280, 800);
            this.MinimumSize = new Size(900, 600);
            this.BackColor = clrBackground;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Application;
            this.DoubleBuffered = true;

            string coverFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CoverImages");
            if (!Directory.Exists(coverFolder)) Directory.CreateDirectory(coverFolder);

            InitializeUI();

            searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 300 };
            searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            DataManager.Instance.SetCurrentUser(0);
            UpdateUIAuth();
        }

        #region UI Initialization

        private void InitializeUI()
        {
            SetupTopBar();
            SetupSidebar();
            SetupContentArea();
            SetupStatusStrip();

            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);
            this.Controls.Add(topBar);
            this.Controls.Add(statusStrip);

            if (sidebarButtons.ContainsKey("Books")) SetActiveButton(sidebarButtons["Books"]);
        }

        private void SetupTopBar()
        {
            topBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = clrTopBar, Padding = new Padding(10) };

            // 1. Menu Button
            menuButton = CreateIconButton("☰", 15, 20, 40, 30);
            menuButton.Click += (s, e) => ToggleSidebar();

            // 2. Logo
            logoLabel = new Label
            {
                Text = "koodo",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(60, 18),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            // 3. Search Bar
            SetupSearchBar();

            // 4. Action Buttons
            int btnY = 15;

            // [ĐƠN GIẢN HÓA] Nút Báo cáo - click trực tiếp
            btnReport = CreateModernButton("🖨 Báo cáo", 110, Color.Teal);
            btnReport.Location = new Point(550, btnY);
            btnReport.Visible = false; // Ẩn khi chưa đăng nhập
            btnReport.Click += (s, e) => BtnReportBooks_Click(); // Click trực tiếp vào báo cáo sách

            // [CẬP NHẬT] Nút Sắp xếp với dropdown menu
            sortButton = CreateModernButton("⇅ Sắp xếp", 110, Color.Transparent);
            sortButton.ForeColor = clrTextActive;
            sortButton.BorderColor = Color.FromArgb(80, 80, 80);
            sortButton.BorderSize = 1;
            sortButton.Location = new Point(670, btnY);
            sortButton.Visible = false; // Ẩn khi chưa đăng nhập
            
            // Tạo menu dropdown cho sắp xếp
            sortMenu = new ContextMenuStrip();
            sortMenu.Renderer = new DarkMenuRenderer();
            sortMenu.BackColor = clrTopBar;
            sortMenu.ForeColor = Color.White;

            var sortOptions = new[] {
                ("📅  Ngày thêm", "Date"),
                ("📖  Tên sách", "Book name"),
                ("✍  Tác giả", "Author name"),
                ("📊  Tiến độ đọc", "Reading progress")
            };

            foreach (var option in sortOptions)
            {
                var item = new ToolStripMenuItem(option.Item1);
                item.Tag = option.Item2;
                item.Click += SortMenuItem_Click;
                sortMenu.Items.Add(item);
            }

            sortMenu.Items.Add(new ToolStripSeparator());

            var itemAscending = new ToolStripMenuItem("⬆  Tăng dần");
            itemAscending.Tag = "ASC";
            itemAscending.Click += SortDirectionMenuItem_Click;
            sortMenu.Items.Add(itemAscending);

            var itemDescending = new ToolStripMenuItem("⬇  Giảm dần");
            itemDescending.Tag = "DESC";
            itemDescending.Click += SortDirectionMenuItem_Click;
            sortMenu.Items.Add(itemDescending);

            sortButton.Click += (s, e) => sortMenu.Show(sortButton, new Point(0, sortButton.Height));

            // --- NÚT GỘP: THÊM SÁCH ---
            btnAddBook = CreateModernButton("➕ Thêm sách", 120, Color.FromArgb(0, 90, 160));
            btnAddBook.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddBook.Location = new Point(this.Width - 260, btnY);
            btnAddBook.Visible = false; // Ẩn khi chưa login

            // Tạo Menu con cho nút Thêm sách
            importMenu = new ContextMenuStrip();
            importMenu.Renderer = new DarkMenuRenderer();
            importMenu.BackColor = clrTopBar;
            importMenu.ForeColor = Color.White;

            var itemImport = new ToolStripMenuItem("📄  Nhập file (Epub, PDF...)");
            itemImport.Click += (s, e) => PerformImportFile();

            var itemScan = new ToolStripMenuItem("📂  Quét thư mục");
            itemScan.Click += (s, e) => PerformScanFolder();

            importMenu.Items.Add(itemImport);
            importMenu.Items.Add(itemScan);

            btnAddBook.Click += (s, e) => importMenu.Show(btnAddBook, new Point(0, btnAddBook.Height));

            // 5. User Profile
            SetupUserProfile();


            topBar.Controls.AddRange(new Control[] {
                menuButton, logoLabel, searchPanel,
                btnReport, sortButton, btnAddBook,
                userButton, lblUsername
            });
        }

        private void SetupSearchBar()
        {
            searchPanel = new Panel
            {
                Location = new Point(200, 15),
                Size = new Size(320, 40),
                BackColor = Color.FromArgb(50, 50, 55),
                Cursor = Cursors.IBeam
            };

            searchPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(new Rectangle(0, 0, searchPanel.Width - 1, searchPanel.Height - 1), 20))
                using (Pen pen = new Pen(Color.FromArgb(70, 70, 70), 1))
                {
                    searchPanel.Region = new Region(path);
                    e.Graphics.DrawPath(pen, path);
                }
            };
            searchPanel.Click += (s, e) => searchBox.Focus();

            searchIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI Emoji", 12),
                ForeColor = clrTextInactive,
                Location = new Point(10, 8),
                Size = new Size(30, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            searchIcon.Click += (s, e) => searchBox.Focus();

            searchBox = new TextBox
            {
                Location = new Point(45, 10),
                Size = new Size(260, 25),
                BackColor = Color.FromArgb(50, 50, 55),
                ForeColor = clrTextInactive,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                Text = "Tìm kiếm sách, tác giả..."
            };

            searchBox.GotFocus += (s, e) => { if (searchBox.Text == "Tìm kiếm sách, tác giả...") { searchBox.Text = ""; searchBox.ForeColor = Color.White; } };
            searchBox.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(searchBox.Text)) { searchBox.Text = "Tìm kiếm sách, tác giả..."; searchBox.ForeColor = clrTextInactive; } };

            searchBox.TextChanged += (s, e) => {
                searchDebounceTimer.Stop();
                if (searchBox.Text != "Tìm kiếm sách, tác giả...") searchDebounceTimer.Start();
            };

            searchPanel.Controls.Add(searchIcon);
            searchPanel.Controls.Add(searchBox);
        }

        private void SetupUserProfile()
        {
            userButton = new Button
            {
                Text = "👤",
                Size = new Size(46, 46),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 64, 67),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 18),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            userButton.FlatAppearance.BorderSize = 0;
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, 46, 46);
            userButton.Region = new Region(gp);
            userButton.Click += UserButton_Click;

            lblUsername = new Label
            {
                Text = "",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };

            authMenu = new ContextMenuStrip
            {
                BackColor = clrTopBar,
                ForeColor = Color.White,
                Renderer = new DarkMenuRenderer()
            };
        }

        private void SetupSidebar()
        {
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = clrSidebar,
                Padding = new Padding(0, 20, 0, 0)
            };

            int yPos = 20;
            AddSidebarButton("Books", "📚 Sách", ref yPos);
            AddSidebarButton("Favorites", "❤️ Yêu thích", ref yPos);
            AddSidebarButton("Notes", "💡 Ghi chú", ref yPos);
            AddSidebarButton("Highlights", "⭐ Đánh dấu", ref yPos);
            AddSidebarButton("Trash", "🗑️ Thùng rác", ref yPos);

            yPos += 20;

            btnShelfToggle = new Button
            {
                Text = "📚  Kệ sách",
                Tag = "Kệ sách",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = clrTextInactive,
                Location = new Point(10, yPos),
                Size = new Size(230, 45),
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Padding = new Padding(10, 0, 0, 0)
            };
            btnShelfToggle.FlatAppearance.BorderSize = 0;
            btnShelfToggle.FlatAppearance.MouseOverBackColor = clrHover;
            btnShelfToggle.Click += (s, e) => ToggleShelf();

            pnlShelfContainer = new FlowLayoutPanel
            {
                Location = new Point(0, yPos + 50),
                Size = new Size(250, 300),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Visible = true,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 0, 0, 0)
            };

            sidebarPanel.Controls.Add(btnShelfToggle);
            sidebarPanel.Controls.Add(pnlShelfContainer);
        }

        private void SetupContentArea()
        {
            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = clrBackground };

            pnlFilterBar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = clrBackground, Visible = false };
            var lbl = new Label { Text = "Lọc theo sách:", ForeColor = clrTextInactive, AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, 15) };

            cmbFilterBook = new ComboBox
            {
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(50, 50, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9),
                Location = new Point(120, 12)
            };
            cmbFilterBook.SelectedIndexChanged += (s, e) => { if (currentView == "Highlights") LoadHighlightsView(); else if (currentView == "Notes") LoadNotesView(); };

            Panel rightFilterPanel = new Panel { Dock = DockStyle.Right, Width = 350, BackColor = Color.Transparent };
            rightFilterPanel.Controls.Add(lbl);
            rightFilterPanel.Controls.Add(cmbFilterBook);
            pnlFilterBar.Controls.Add(rightFilterPanel);

            booksPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = clrBackground,
                Padding = new Padding(30)
            };

            contentPanel.Controls.Add(booksPanel);
            contentPanel.Controls.Add(pnlFilterBar);
        }

        private void SetupStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusStrip.BackColor = clrSidebar;
            statusStrip.ForeColor = clrTextInactive;
            statusStrip.SizingGrip = false;

            lblStatusState = new ToolStripStatusLabel("Sẵn sàng");
            lblStatusTotal = new ToolStripStatusLabel("");
            lblStatusTotal.Spring = true;
            lblStatusTotal.TextAlign = ContentAlignment.MiddleRight;

            statusStrip.Items.Add(lblStatusState);
            statusStrip.Items.Add(lblStatusTotal);
        }

        #endregion

        #region Logic & Event Handlers

        private void AddSidebarButton(string key, string text, ref int yPos)
        {
            Button btn = new Button
            {
                Text = text,
                Tag = text,
                Location = new Point(10, yPos),
                Size = new Size(230, 45),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                ForeColor = clrTextInactive,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = clrHover }
            };
            btn.Click += (s, e) => SwitchView(key);
            sidebarButtons.Add(key, btn);
            sidebarPanel.Controls.Add(btn);
            yPos += 55;
        }

        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            searchDebounceTimer.Stop();
            if (currentView == "Highlights") LoadHighlightsView();
            else if (currentView == "Notes") LoadNotesView();
            else LoadBooks();
        }

        private void SwitchView(string view)
        {
            if (_currentUser == null && view != "Books") return;

            currentView = view;

            bool isFilterView = view == "Highlights" || view == "Notes";
            pnlFilterBar.Visible = isFilterView;
            
            // [CẬP NHẬT] Chỉ hiện nút Sắp xếp và Báo cáo khi ở view Books hoặc các view liên quan đến sách
            bool isBookView = (view == "Books" || view == "Favorites" || view == "Trash" || view == "Shelf");
            sortButton.Visible = isBookView && _currentUser != null;
            btnReport.Visible = (view == "Books") && _currentUser != null; // Chỉ hiện ở Books

            if (isFilterView) LoadFilterCombobox();

            if (sidebarButtons.ContainsKey(view)) SetActiveButton(sidebarButtons[view]);
            else if (view == "Shelf") SetActiveButton(null);

            switch (view)
            {
                case "Books": LoadBooks(); break;
                case "Favorites": LoadBooks(); break;
                case "Highlights": LoadHighlightsView(); break;
                case "Notes": LoadNotesView(); break;
                case "Trash": LoadBooks(); break;
                case "Shelf": LoadBooks(); break;
            }
        }

        private async void LoadBooks()
        {
            SetLoadingState(true, "Đang tải dữ liệu...");
            booksPanel.Controls.Clear();

            if (_currentUser == null)
            {
                SetLoadingState(false, "Vui lòng đăng nhập");
                return;
            }

            List<Book> books = await Task.Run(() =>
            {
                List<Book> result;
                if (currentView == "Trash") result = DataManager.Instance.GetDeletedBooks();
                else if (currentView == "Favorites") result = DataManager.Instance.GetFavoriteBooks();
                else if (currentView == "Shelf") result = DataManager.Instance.GetBooksByShelf(activeShelfId);
                else result = DataManager.Instance.GetAllBooks();

                string query = "";
                this.Invoke((MethodInvoker)(() => query = searchBox.Text.Trim().ToLower()));

                if (!string.IsNullOrEmpty(query) && query != "tìm kiếm sách, tác giả...")
                {
                    result = result.Where(b => b.Title.ToLower().Contains(query) || b.Author.ToLower().Contains(query)).ToList();
                }

                ApplySort(ref result);
                return result;
            });

            booksPanel.SuspendLayout();
            foreach (var book in books)
            {
                var bookCard = new BookCard { Book = book, Margin = new Padding(15) };
                bookCard.BookClicked += (s, e) => OpenBook(book);
                bookCard.MenuClicked += (s, e) => ShowBookMenu(book, bookCard);
                booksPanel.Controls.Add(bookCard);
            }
            booksPanel.ResumeLayout();

            SetLoadingState(false, $"Tổng {books.Count} cuốn");
        }

        private void SetLoadingState(bool isLoading, string statusText)
        {
            this.Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            lblStatusState.Text = isLoading ? "Đang xử lý..." : "Sẵn sàng";
            lblStatusTotal.Text = statusText;
        }

        private void ToggleShelf()
        {
            if (!isSidebarExpanded) return;
            isShelfExpanded = !isShelfExpanded;
            pnlShelfContainer.Visible = isShelfExpanded;
        }

        private void RefreshSidebarShelves()
        {
            pnlShelfContainer.Controls.Clear();
            if (_currentUser == null) return;

            Button btnNew = CreateSidebarSubButton("➕  Kệ mới");
            btnNew.ForeColor = Color.FromArgb(76, 175, 80);
            btnNew.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnNew.Click += BtnAddShelf_Click;
            pnlShelfContainer.Controls.Add(btnNew);

            Button btnManage = CreateSidebarSubButton("⚙️  Quản lý kệ");
            btnManage.ForeColor = clrAccent;
            btnManage.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnManage.Click += BtnManageShelf_Click;
            pnlShelfContainer.Controls.Add(btnManage);

            var separator = new Panel { Height = 1, Width = 200, BackColor = Color.FromArgb(60, 60, 63), Margin = new Padding(5, 8, 5, 8) };
            pnlShelfContainer.Controls.Add(separator);

            var shelves = DataManager.Instance.GetShelvesList();
            foreach (var shelf in shelves)
            {
                Button btnShelf = CreateSidebarSubButton("📖  " + shelf.Name);
                btnShelf.Tag = shelf.Id;
                btnShelf.Click += (s, e) =>
                {
                    activeShelfId = shelf.Id;
                    foreach (Control c in pnlShelfContainer.Controls)
                        if (c is Button b && b.Tag is int) { b.ForeColor = clrTextInactive; b.BackColor = Color.Transparent; }

                    btnShelf.ForeColor = Color.White;
                    btnShelf.BackColor = clrHover;
                    SwitchView("Shelf");
                };
                pnlShelfContainer.Controls.Add(btnShelf);
            }
        }

        // --- RESTORED: LOGIC HIỂN THỊ HIGHLIGHT/NOTE CÓ NÚT XÓA ---
        private void LoadHighlightsView()
        {
            booksPanel.Controls.Clear();
            SetLoadingState(false, "Danh sách Đánh dấu");
            if (_currentUser == null) return;
            var highlights = DataManager.Instance.GetOnlyHighlights(_currentUser.Id);

            if (cmbFilterBook.Visible && cmbFilterBook.SelectedValue != null && int.TryParse(cmbFilterBook.SelectedValue.ToString(), out int selectedBookId) && selectedBookId != -1)
                highlights = highlights.Where(h => h.BookId == selectedBookId).ToList();

            string query = searchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(query) && query != "tìm kiếm sách, tác giả...")
                highlights = highlights.Where(h => h.BookTitle.ToLower().Contains(query) || h.SelectedText.ToLower().Contains(query)).ToList();

            foreach (var hl in highlights) { Panel card = CreateInfoCard(hl, false); booksPanel.Controls.Add(card); }
        }

        private void LoadNotesView()
        {
            booksPanel.Controls.Clear();
            SetLoadingState(false, "Danh sách Ghi chú");
            if (_currentUser == null) return;
            var notes = DataManager.Instance.GetOnlyNotes(_currentUser.Id);

            if (cmbFilterBook.Visible && cmbFilterBook.SelectedValue != null && int.TryParse(cmbFilterBook.SelectedValue.ToString(), out int selectedBookId) && selectedBookId != -1)
                notes = notes.Where(n => n.BookId == selectedBookId).ToList();

            string query = searchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(query) && query != "tìm kiếm sách, tác giả...")
                notes = notes.Where(n => n.BookTitle.ToLower().Contains(query) || n.Note.ToLower().Contains(query) || n.SelectedText.ToLower().Contains(query)).ToList();

            foreach (var note in notes) { Panel card = CreateInfoCard(note, true); booksPanel.Controls.Add(card); }
        }

        // [KHÔI PHỤC] Hàm tạo thẻ Note/Highlight có đầy đủ chức năng Xóa và Đi tới
        private Panel CreateInfoCard(Highlight item, bool isNote)
        {
            Panel card = new Panel
            {
                Size = new Size(booksPanel.Width - 60, isNote ? 140 : 100),
                BackColor = clrCardBg,
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };

            // Thanh màu bên trái
            Panel colorBar = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = ColorTranslator.FromHtml(item.ColorHex) };

            // Tên sách
            Label lblBook = new Label
            {
                Text = "📖 " + item.BookTitle,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                Location = new Point(15, 10),
                AutoSize = true
            };

            // Nội dung trích dẫn
            Label lblQuote = new Label
            {
                Text = $"\"{item.SelectedText}\"",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, isNote ? FontStyle.Regular : FontStyle.Bold),
                Location = new Point(15, 35),
                Size = new Size(card.Width - 140, 40),
                AutoEllipsis = true
            };

            // Nút "Đi tới" (Jump)
            Button btnJump = new Button
            {
                Text = "Đi tới ➔",
                Size = new Size(70, 30),
                Location = new Point(card.Width - 120, 10),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnJump.FlatAppearance.BorderSize = 0;
            btnJump.Click += (s, e) => {
                var book = DataManager.Instance.GetBookById(item.BookId);
                if (book != null)
                {
                    BookReaderForm reader = new BookReaderForm(book, item.ChapterIndex, item.StartIndex);
                    reader.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy cuốn sách này (có thể đã bị xóa).");
                }
            };

            // Nút Xóa (Delete) - Đã khôi phục
            Button btnDelete = new Button
            {
                Text = "🗑",
                Size = new Size(40, 30),
                Location = new Point(card.Width - 45, 10),
                BackColor = Color.Transparent,
                ForeColor = Color.IndianRed,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 11)
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);

            btnDelete.Click += (s, e) => {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa mục này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        DataManager.Instance.DeleteHighlight(item.Id);
                        if (isNote) LoadNotesView(); else LoadHighlightsView();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xóa: " + ex.Message);
                    }
                }
            };

            card.Controls.AddRange(new Control[] { btnDelete, btnJump, lblQuote, lblBook, colorBar });

            // Nếu là Note thì hiện thêm phần ghi chú của người dùng
            if (isNote)
            {
                Label lblUserNote = new Label
                {
                    Text = "📝 " + item.Note,
                    ForeColor = Color.Yellow,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(15, 80),
                    Size = new Size(card.Width - 30, 50),
                    AutoEllipsis = true
                };
                card.Controls.Add(lblUserNote);
            }
            else
            {
                // Click vào card thì nhảy tới trang
                card.Click += (s, e) => btnJump.PerformClick();
                lblQuote.Click += (s, e) => btnJump.PerformClick();
            }

            return card;
        }

        private void LoadFilterCombobox() { if (_currentUser == null) return; List<Book> books = (currentView == "Highlights") ? DataManager.Instance.GetBooksWithHighlights() : (currentView == "Notes" ? DataManager.Instance.GetBooksWithNotes() : DataManager.Instance.GetAllBooks()); var defaultOption = new Book { Id = -1, Title = "Tất cả sách" }; books.Insert(0, defaultOption); cmbFilterBook.DataSource = books; cmbFilterBook.DisplayMember = "Title"; cmbFilterBook.ValueMember = "Id"; }
        private void ApplySort(ref List<Book> books) { switch (currentSortBy) { case "Vừa đọc": case "Recently read": case "Ngày thêm": case "Date": books = sortAscending ? books.OrderBy(b => b.DateAdded).ToList() : books.OrderByDescending(b => b.DateAdded).ToList(); break; case "Tên sách": case "Book name": books = sortAscending ? books.OrderBy(b => b.Title).ToList() : books.OrderByDescending(b => b.Title).ToList(); break; case "Tác giả": case "Author name": books = sortAscending ? books.OrderBy(b => b.Author).ToList() : books.OrderByDescending(b => b.Author).ToList(); break; case "Tiến độ đọc": case "Reading progress": books = sortAscending ? books.OrderBy(b => b.Progress).ToList() : books.OrderByDescending(b => b.Progress).ToList(); break; default: books = books.OrderByDescending(b => b.DateAdded).ToList(); break; } }

        // --- CÁC HÀM XỬ LÝ NHẬP LIỆU ---

        private async void PerformScanFolder()
        {
            if (_currentUser == null) return;
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SetLoadingState(true, "Đang quét thư mục...");
                    await Task.Run(() => {
                        new BookScannerService(DataManager.Instance).ScanFolderAndImport(fbd.SelectedPath, _currentUser.Id, (msg) => { });
                    });
                    LoadBooks();
                    MessageBox.Show("Quét hoàn tất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void PerformImportFile()
        {
            if (_currentUser == null) return;
            using (OpenFileDialog ofd = new OpenFileDialog { Multiselect = true, Filter = "Ebooks|*.epub;*.pdf;*.txt;*.mobi" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    SetLoadingState(true, "Đang nhập sách...");
                    int count = 0;
                    List<string> errorFiles = new List<string>();
                    var scanner = new BookScannerService(DataManager.Instance);

                    await Task.Run(() =>
                    {
                        foreach (var f in ofd.FileNames)
                        {
                            try
                            {
                                if (DataManager.Instance.IsBookExists(f)) continue;
                                var book = scanner.CreateBookFromFile(f);
                                if (book != null) { DataManager.Instance.AddBook(book); count++; }
                                else errorFiles.Add(Path.GetFileName(f));
                            }
                            catch { errorFiles.Add(Path.GetFileName(f)); }
                        }
                    });

                    if (count > 0) MessageBox.Show($"Đã thêm {count} sách!");
                    if (errorFiles.Count > 0) MessageBox.Show($"Lỗi {errorFiles.Count} file:\n" + string.Join("\n", errorFiles.Take(5)) + "...");
                    LoadBooks();
                }
            }
        }

        // --- BUTTON EVENTS ---

        private void BtnReport_Click(object sender, EventArgs e)
        {
            if (_currentUser == null) { MessageBox.Show("Vui lòng đăng nhập!", "Yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var books = DataManager.Instance.GetAllBooks();
            if (books.Count == 0) { MessageBox.Show("Không có dữ liệu để báo cáo.", "Thông báo"); return; }
            var reportService = new WindowsFormsApp1.Services.ReportService();
            reportService.CreateBookListReport(books, _currentUser.DisplayName);
        }

        private void ToggleSidebar()
        {
            isSidebarExpanded = !isSidebarExpanded;
            sidebarPanel.Width = isSidebarExpanded ? 250 : 70;
            foreach (var btn in sidebarButtons.Values) UpdateButtonText(btn, isSidebarExpanded);
            btnShelfToggle.Text = isSidebarExpanded ? " " + btnShelfToggle.Tag.ToString() : "📚";
            pnlShelfContainer.Visible = isSidebarExpanded && isShelfExpanded;
            logoLabel.Visible = isSidebarExpanded;
        }

        private void SetActiveButton(Button activeBtn)
        {
            foreach (var btn in sidebarButtons.Values)
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = clrTextInactive;
            }
            if (activeBtn != null)
            {
                activeBtn.BackColor = Color.FromArgb(50, 50, 55);
                activeBtn.ForeColor = clrAccent;
            }
        }

        private void UpdateUIAuth()
        {
            int rightMargin = 20; int gap = 15;
            userButton.Location = new Point(topBar.Width - userButton.Width - rightMargin, 12);
            userButton.Visible = true;

            if (_currentUser == null)
            {
                // [CẬP NHẬT] Ẩn các nút khi chưa đăng nhập
                btnAddBook.Visible = false;
                btnReport.Visible = false;
                sortButton.Visible = false;
                lblUsername.Visible = false;
                userButton.BackColor = Color.FromArgb(80, 80, 80);
                userButton.Text = "👤";
            }
            else
            {
                userButton.BackColor = Color.FromArgb(234, 67, 53);
                userButton.Text = _currentUser.DisplayName.Length > 0 ? _currentUser.DisplayName.Substring(0, 1).ToUpper() : "U";

                lblUsername.Text = _currentUser.DisplayName;
                lblUsername.Visible = true;
                lblUsername.Location = new Point(userButton.Left - lblUsername.Width - gap, 25);

                // [CẬP NHẬT] Hiện các nút khi đã đăng nhập
                btnAddBook.Visible = true;
                // btnReport và sortButton sẽ được điều khiển bởi SwitchView()
                // Chỉ hiện sortButton nếu đang ở view sách
                bool isBookView = (currentView == "Books" || currentView == "Favorites" || currentView == "Trash" || currentView == "Shelf");
                sortButton.Visible = isBookView;
                btnReport.Visible = (currentView == "Books"); // Chỉ hiện ở Books
                
                btnAddBook.Location = new Point(lblUsername.Left - btnAddBook.Width - gap, 15);
                sortButton.Location = new Point(btnAddBook.Left - sortButton.Width - gap, 15);
                btnReport.Location = new Point(sortButton.Left - btnReport.Width - gap, 15);
            }
            RefreshSidebarShelves();
        }

        // [MỚI] Xử lý sự kiện click vào menu sắp xếp
        private void SortMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag != null)
            {
                currentSortBy = item.Tag.ToString();
                LoadBooks();
            }
        }

        private void SortDirectionMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag != null)
            {
                sortAscending = item.Tag.ToString() == "ASC";
                LoadBooks();
            }
        }

        // [MỚI] Báo cáo sách
        private void BtnReportBooks_Click()
        {
            if (_currentUser == null) 
            { 
                MessageBox.Show("Vui lòng đăng nhập!", "Yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return; 
            }
            
            var books = DataManager.Instance.GetAllBooks();
            if (books.Count == 0) 
            { 
                MessageBox.Show("Không có dữ liệu để báo cáo.", "Thông báo"); 
                return; 
            }
            
            var reportService = new WindowsFormsApp1.Services.ReportService();
            reportService.CreateBookListReport(books, _currentUser.DisplayName);
        }

        private void ShowLoginForm() { var form = new LoginForm(); if (form.ShowDialog() == DialogResult.OK) { _currentUser = form.LoggedInUser; UpdateUIAuth(); LoadBooks(); } }
        private void ShowRegisterForm() { var form = new RegisterForm(); if (form.ShowDialog() == DialogResult.OK) { _currentUser = form.RegisteredUser; UpdateUIAuth(); LoadBooks(); } }
        private void OpenBook(Book book) { if (!File.Exists(book.FilePath)) { MessageBox.Show("File không tồn tại"); return; } var form = new BookReaderForm(book); form.ShowDialog(); LoadBooks(); }

        private void ShowBookMenu(Book book, BookCard card)
        {
            ContextMenuStrip menu = new ContextMenuStrip { BackColor = clrTopBar, ForeColor = Color.White, Renderer = new DarkMenuRenderer() };
            if (!book.IsDeleted)
            {
                var editItem = new ToolStripMenuItem("✎  Sửa thông tin"); editItem.Click += (s, e) => { using (var editForm = new EditBookForm(book)) { if (editForm.ShowDialog() == DialogResult.OK) LoadBooks(); } }; menu.Items.Add(editItem);
                menu.Items.Add(new ToolStripSeparator());
                var openItem = new ToolStripMenuItem("📁  Mở thư mục"); openItem.Click += (s, e) => System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{book.FilePath}\""); menu.Items.Add(openItem);
                var delItem = new ToolStripMenuItem("🗑️  Xóa"); delItem.Click += (s, e) => { DataManager.Instance.DeleteBook(book.Id); LoadBooks(); }; menu.Items.Add(delItem);
            }
            else
            {
                var restore = new ToolStripMenuItem("♻️  Khôi phục"); restore.Click += (s, e) => { DataManager.Instance.RestoreBook(book.Id); LoadBooks(); }; menu.Items.Add(restore);
            }
            menu.Show(card, new Point(0, card.Height));
        }

        private void BtnAddShelf_Click(object sender, EventArgs e) { if (_currentUser == null) return; using (var dlg = new AddShelfDialog()) if (dlg.ShowDialog() == DialogResult.OK) { DataManager.Instance.AddShelf(dlg.ShelfName, dlg.ShelfDescription); RefreshSidebarShelves(); } }
        private void BtnManageShelf_Click(object sender, EventArgs e) { if (_currentUser == null) return; using (var dlg = new ManageShelfDialog()) { dlg.ShowDialog(); RefreshSidebarShelves(); } }
        private void SortButton_Click(object sender, EventArgs e) { /* Sort logic */ }

        private void UserButton_Click(object sender, EventArgs e)
        {
            authMenu.Items.Clear();
            if (_currentUser == null)
            {
                authMenu.Items.Add("🔑  Đăng Nhập", null, (s, ev) => ShowLoginForm());
                authMenu.Items.Add("📝  Đăng Ký", null, (s, ev) => ShowRegisterForm());
            }
            else
            {
                var editProfile = new ToolStripMenuItem($"👤  {_currentUser.DisplayName} (Sửa)");
                editProfile.Click += (s, ev) => { using (var pwd = new PasswordPromptForm()) { if (pwd.ShowDialog() == DialogResult.OK && pwd.IsVerified) { using (var edit = new EditProfileForm(_currentUser)) { if (edit.ShowDialog() == DialogResult.OK && edit.UpdatedUser != null) { _currentUser = edit.UpdatedUser; UpdateUIAuth(); } } } } };
                authMenu.Items.Add(editProfile);
                authMenu.Items.Add(new ToolStripSeparator());
                authMenu.Items.Add("🚪  Đăng Xuất", null, (s, ev) => { _currentUser = null; DataManager.Instance.SetCurrentUser(0); booksPanel.Controls.Clear(); UpdateUIAuth(); });
            }
            authMenu.Show(userButton, new Point(0, userButton.Height));
        }

        private ModernButton CreateModernButton(string text, int width, Color backColor)
        {
            return new ModernButton
            {
                Text = text,
                Size = new Size(width, 40),
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 20,
                HoverColor = ControlPaint.Light(backColor)
            };
        }

        private Button CreateIconButton(string text, int x, int y, int width, int height) => new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, height),
            BackColor = Color.Transparent,
            ForeColor = clrTextInactive,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 14),
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        private Button CreateSidebarSubButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(210, 35),
                BackColor = Color.Transparent,
                ForeColor = clrTextInactive,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2),
                Padding = new Padding(10, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = clrHover;
            return btn;
        }

        private GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();
            if (radius == 0) { path.AddRectangle(bounds); return path; }
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void UpdateButtonText(Button btn, bool show)
        {
            // Placeholder implementation - can be enhanced based on requirements
            if (!show && btn.Tag != null)
            {
                string fullText = btn.Tag.ToString();
                // Extract emoji only if text contains emoji
                if (fullText.Contains(" "))
                {
                    btn.Text = fullText.Split(' ')[0]; // Get emoji part
                }
            }
            else if (btn.Tag != null)
            {
                btn.Text = btn.Tag.ToString();
            }
        }

        #endregion
    }

    public class ModernButton : Button
    {
        public int BorderRadius { get; set; } = 20;
        public Color BorderColor { get; set; } = Color.Transparent;
        public int BorderSize { get; set; } = 0;
        public Color HoverColor { get; set; } = Color.Gray;
        private Color originalBackColor;

        public ModernButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 40);
            this.BackColor = Color.MediumSlateBlue;
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.Resize += (s, e) => { if (BorderRadius > this.Height) BorderRadius = this.Height; };
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF rectSurface = new RectangleF(0, 0, this.Width, this.Height);
            RectangleF rectBorder = new RectangleF(1, 1, this.Width - 0.8f, this.Height - 1);

            if (BorderRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, BorderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, BorderRadius - 1f))
                using (Pen penSurface = new Pen(this.Parent.BackColor, 2))
                using (Pen penBorder = new Pen(BorderColor, BorderSize))
                {
                    penBorder.Alignment = PenAlignment.Inset;
                    this.Region = new Region(pathSurface);
                    pevent.Graphics.DrawPath(penSurface, pathSurface);
                    if (BorderSize >= 1) pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else
            {
                this.Region = new Region(rectSurface);
                if (BorderSize >= 1) { using (Pen penBorder = new Pen(BorderColor, BorderSize)) { penBorder.Alignment = PenAlignment.Inset; pevent.Graphics.DrawRectangle(penBorder, 0, 0, this.Width - 1, this.Height - 1); } }
            }
        }

        private GraphicsPath GetFigurePath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); originalBackColor = this.BackColor; this.BackColor = HoverColor; }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); this.BackColor = originalBackColor; }
    }

    public class DarkMenuRenderer : ToolStripProfessionalRenderer { public DarkMenuRenderer() : base(new DarkMenuColors()) { } }
    public class DarkMenuColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Color.FromArgb(60, 60, 63);
        public override Color MenuItemBorder => Color.Transparent;
        public override Color MenuBorder => Color.FromArgb(60, 60, 63);
        public override Color ToolStripDropDownBackground => Color.FromArgb(40, 41, 45);
        public override Color ImageMarginGradientBegin => Color.FromArgb(40, 41, 45);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(40, 41, 45);
        public override Color ImageMarginGradientEnd => Color.FromArgb(40, 41, 45);
    }
}