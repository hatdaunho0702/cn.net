using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1.Forms
{
    public partial class ReadingGoalsForm : Form
    {
        private readonly int userId;
        private Label lblTodayMinutes;
        private Label lblMonthlyBooks;
        private Label lblYearlyBooks;
        private Label lblCurrentStreak;
        private Label lblLongestStreak;
        private ProgressBar progDailyGoal;
        private ProgressBar progMonthlyGoal;
        private ProgressBar progYearlyGoal;
        private Button btnSetDailyGoal;
        private Button btnSetMonthlyGoal;
        private Button btnSetYearlyGoal;
        private Panel pnlWeeklyChart;

        public ReadingGoalsForm(int userId)
        {
            this.userId = userId;
            InitializeComponent();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = "M?c tiêu ð?c sách";
            this.Size = new Size(900, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Font = new Font("Segoe UI", 9F);

            // === HEADER ===
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            Label lblHeader = new Label
            {
                Text = "TH?NG KÊ Ð?C SÁCH",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 20)
            };
            pnlHeader.Controls.Add(lblHeader);

            // Button xu?t báo cáo
            Button btnExportReport = CreateHeaderButton("XU?T BÁO CÁO", 720, 25);
            btnExportReport.Click += BtnExportReport_Click;
            pnlHeader.Controls.Add(btnExportReport);

            this.Controls.Add(pnlHeader);

            // === MAIN CONTAINER ===
            FlowLayoutPanel mainContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(25, 25, 25, 10)
            };

            // === 1. STREAK CARD ===
            Panel pnlStreak = CreateModernCard("CHU?I NGÀY Ð?C LIÊN T?C", Color.FromArgb(231, 76, 60));
            
            FlowLayoutPanel streakContent = new FlowLayoutPanel
            {
                Location = new Point(30, 60),
                Size = new Size(780, 80),
                FlowDirection = FlowDirection.LeftToRight
            };

            Panel streakLeft = new Panel { Width = 400, Height = 80 };
            lblCurrentStreak = new Label
            {
                Text = "0 ngày",
                Font = new Font("Segoe UI", 42, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            streakLeft.Controls.Add(lblCurrentStreak);

            Panel streakRight = new Panel { Width = 350, Height = 80 };
            lblLongestStreak = new Label
            {
                Text = "K? l?c: 0 ngày",
                Font = new Font("Segoe UI", 13),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(0, 30)
            };
            streakRight.Controls.Add(lblLongestStreak);

            streakContent.Controls.Add(streakLeft);
            streakContent.Controls.Add(streakRight);
            pnlStreak.Controls.Add(streakContent);
            mainContainer.Controls.Add(pnlStreak);

            // === 2. DAILY GOAL CARD ===
            Panel pnlDaily = CreateModernCard("M?C TIÊU HÔM NAY", Color.FromArgb(46, 204, 113));
            
            lblTodayMinutes = new Label
            {
                Text = "0 / 30 phút",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(30, 60),
                AutoSize = true
            };
            pnlDaily.Controls.Add(lblTodayMinutes);

            progDailyGoal = CreateModernProgressBar(30, 95, Color.FromArgb(46, 204, 113));
            pnlDaily.Controls.Add(progDailyGoal);

            btnSetDailyGoal = CreateModernButton("Ð?t m?c tiêu", Color.FromArgb(46, 204, 113), 650, 55);
            btnSetDailyGoal.Click += BtnSetDailyGoal_Click;
            pnlDaily.Controls.Add(btnSetDailyGoal);

            mainContainer.Controls.Add(pnlDaily);

            // === 3. MONTHLY GOAL CARD ===
            Panel pnlMonthly = CreateModernCard("M?C TIÊU THÁNG NÀY", Color.FromArgb(241, 196, 15));
            
            lblMonthlyBooks = new Label
            {
                Text = "0 / 3 cu?n",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15),
                Location = new Point(30, 60),
                AutoSize = true
            };
            pnlMonthly.Controls.Add(lblMonthlyBooks);

            progMonthlyGoal = CreateModernProgressBar(30, 95, Color.FromArgb(241, 196, 15));
            pnlMonthly.Controls.Add(progMonthlyGoal);

            btnSetMonthlyGoal = CreateModernButton("Ð?t m?c tiêu", Color.FromArgb(241, 196, 15), 650, 55);
            btnSetMonthlyGoal.Click += BtnSetMonthlyGoal_Click;
            pnlMonthly.Controls.Add(btnSetMonthlyGoal);

            mainContainer.Controls.Add(pnlMonthly);

            // === 4. YEARLY GOAL CARD ===
            Panel pnlYearly = CreateModernCard("M?C TIÊU NÃM NAY", Color.FromArgb(155, 89, 182));
            
            lblYearlyBooks = new Label
            {
                Text = "0 / 12 cu?n",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182),
                Location = new Point(30, 60),
                AutoSize = true
            };
            pnlYearly.Controls.Add(lblYearlyBooks);

            progYearlyGoal = CreateModernProgressBar(30, 95, Color.FromArgb(155, 89, 182));
            pnlYearly.Controls.Add(progYearlyGoal);

            btnSetYearlyGoal = CreateModernButton("Ð?t m?c tiêu", Color.FromArgb(155, 89, 182), 650, 55);
            btnSetYearlyGoal.Click += BtnSetYearlyGoal_Click;
            pnlYearly.Controls.Add(btnSetYearlyGoal);

            mainContainer.Controls.Add(pnlYearly);

            // === 5. WEEKLY CHART ===
            Panel pnlWeekly = CreateModernCard("TH?NG KÊ 7 NGÀY G?N NH?T", Color.FromArgb(52, 152, 219));
            pnlWeekly.Height = 200;
            
            pnlWeeklyChart = new Panel
            {
                Location = new Point(30, 60),
                Size = new Size(800, 120),
                BackColor = Color.Transparent
            };
            pnlWeekly.Controls.Add(pnlWeeklyChart);

            mainContainer.Controls.Add(pnlWeekly);

            this.Controls.Add(mainContainer);
        }

        private Button CreateHeaderButton(string text, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 152, 219);

            GraphicsPath path = new GraphicsPath();
            int radius = 6;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            btn.Region = new Region(path);

            return btn;
        }

        private Panel CreateModernCard(string title, Color accentColor)
        {
            Panel card = new Panel
            {
                Size = new Size(840, 150),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 20)
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    Rectangle shadowRect = new Rectangle(3, 3, card.Width - 3, card.Height - 3);
                    e.Graphics.FillRectangle(shadowBrush, shadowRect);
                }

                using (SolidBrush cardBrush = new SolidBrush(Color.White))
                {
                    Rectangle cardRect = new Rectangle(0, 0, card.Width - 3, card.Height - 3);
                    e.Graphics.FillRectangle(cardBrush, cardRect);
                }

                using (SolidBrush accentBrush = new SolidBrush(accentColor))
                {
                    Rectangle accentRect = new Rectangle(0, 0, 5, card.Height - 3);
                    e.Graphics.FillRectangle(accentBrush, accentRect);
                }

                using (Pen borderPen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    Rectangle borderRect = new Rectangle(0, 0, card.Width - 4, card.Height - 4);
                    e.Graphics.DrawRectangle(borderPen, borderRect);
                }
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 15),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            return card;
        }

        private ProgressBar CreateModernProgressBar(int x, int y, Color barColor)
        {
            ProgressBar prog = new ProgressBar
            {
                Location = new Point(x, y),
                Size = new Size(600, 18),
                Style = ProgressBarStyle.Continuous,
                Maximum = 100,
                Value = 0
            };

            prog.Paint += (s, e) =>
            {
                Rectangle rect = prog.ClientRectangle;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(236, 240, 241)))
                {
                    e.Graphics.FillRectangle(bgBrush, rect);
                }

                if (prog.Value > 0)
                {
                    int width = (int)(rect.Width * ((double)prog.Value / prog.Maximum));
                    Rectangle progressRect = new Rectangle(0, 0, width, rect.Height);
                    
                    using (LinearGradientBrush gradBrush = new LinearGradientBrush(
                        progressRect, barColor, Color.FromArgb(barColor.A, 
                        Math.Min(255, barColor.R + 30), 
                        Math.Min(255, barColor.G + 30), 
                        Math.Min(255, barColor.B + 30)), 
                        LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(gradBrush, progressRect);
                    }
                }

                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            return prog;
        }

        private Button CreateModernButton(string text, Color bgColor, int x, int y)
        {
            Button btn = new Button
            {
                Location = new Point(x, y),
                Size = new Size(160, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = bgColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Text = "? " + text // Thay ð?i icon t? ? sang ?
            };
            
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Max(0, bgColor.R - 20),
                Math.Max(0, bgColor.G - 20),
                Math.Max(0, bgColor.B - 20)
            );

            GraphicsPath path = new GraphicsPath();
            int radius = 10;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            btn.Region = new Region(path);

            // Shadow effect
            btn.Paint += (s, e) =>
            {
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    Rectangle shadowRect = new Rectangle(2, 2, btn.Width - 2, btn.Height - 2);
                    GraphicsPath shadowPath = new GraphicsPath();
                    shadowPath.AddArc(2, 2, radius, radius, 180, 90);
                    shadowPath.AddArc(shadowRect.Right - radius, 2, radius, radius, 270, 90);
                    shadowPath.AddArc(shadowRect.Right - radius, shadowRect.Bottom - radius, radius, radius, 0, 90);
                    shadowPath.AddArc(2, shadowRect.Bottom - radius, radius, radius, 90, 90);
                    shadowPath.CloseFigure();
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            };

            return btn;
        }

        private void LoadStatistics()
        {
            try
            {
                var streak = DataManager.Instance.GetReadingStreak(userId);
                lblCurrentStreak.Text = $"{streak.CurrentStreak} ngày";
                lblLongestStreak.Text = $"K? l?c: {streak.LongestStreak} ngày";

                int todayMinutes = DataManager.Instance.GetTodayReadingMinutes(userId);
                var dailyGoals = DataManager.Instance.GetActiveGoals(userId)
                    .FirstOrDefault(g => g.GoalType == "DAILY_MINUTES");
                
                int targetMinutes = dailyGoals?.TargetValue ?? 30;
                lblTodayMinutes.Text = $"{todayMinutes} / {targetMinutes} phút";
                progDailyGoal.Value = Math.Min(100, (int)((double)todayMinutes / targetMinutes * 100));

                int monthlyBooks = DataManager.Instance.GetMonthlyBooksRead(userId);
                var monthlyGoals = DataManager.Instance.GetActiveGoals(userId)
                    .FirstOrDefault(g => g.GoalType == "MONTHLY_BOOKS");
                
                int targetMonthlyBooks = monthlyGoals?.TargetValue ?? 3;
                lblMonthlyBooks.Text = $"{monthlyBooks} / {targetMonthlyBooks} cu?n";
                progMonthlyGoal.Value = Math.Min(100, (int)((double)monthlyBooks / targetMonthlyBooks * 100));

                int booksRead = DataManager.Instance.GetYearlyBooksRead(userId);
                var yearlyGoals = DataManager.Instance.GetActiveGoals(userId)
                    .FirstOrDefault(g => g.GoalType == "YEARLY_BOOKS");
                
                int targetBooks = yearlyGoals?.TargetValue ?? 12;
                lblYearlyBooks.Text = $"{booksRead} / {targetBooks} cu?n";
                progYearlyGoal.Value = Math.Min(100, (int)((double)booksRead / targetBooks * 100));

                LoadWeeklyChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i t?i th?ng kê: " + ex.Message, "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWeeklyChart()
        {
            pnlWeeklyChart.Controls.Clear();
            var weeklyStats = DataManager.Instance.GetWeeklyStats(userId);

            int barWidth = 90;
            int maxHeight = 100;
            int spacing = 10;

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var stat = weeklyStats.FirstOrDefault(s => s.Date.Date == date.Date);
                int minutes = stat?.TotalMinutes ?? 0;

                int xPos = (6 - i) * (barWidth + spacing);

                Panel barContainer = new Panel
                {
                    Location = new Point(xPos, 0),
                    Size = new Size(barWidth, maxHeight + 30),
                    BackColor = Color.Transparent
                };

                int barHeight = Math.Min(maxHeight, (int)((double)minutes / 60 * maxHeight));
                
                Panel bar = new Panel
                {
                    Location = new Point(20, maxHeight - barHeight),
                    Size = new Size(50, barHeight),
                    BackColor = minutes >= 30 ? Color.FromArgb(46, 204, 113) : Color.FromArgb(189, 195, 199)
                };
                
                bar.Paint += (s, e) =>
                {
                    Rectangle rect = bar.ClientRectangle;
                    using (SolidBrush brush = new SolidBrush(bar.BackColor))
                    {
                        GraphicsPath path = new GraphicsPath();
                        int radius = 6;
                        path.AddArc(0, 0, radius, radius, 180, 90);
                        path.AddArc(rect.Width - radius, 0, radius, radius, 270, 90);
                        path.AddLine(rect.Width, radius, rect.Width, rect.Height);
                        path.AddLine(0, rect.Height, 0, radius);
                        path.CloseFigure();
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.FillPath(brush, path);
                    }
                };

                barContainer.Controls.Add(bar);

                Label lblMinutes = new Label
                {
                    Text = minutes + "p",
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    ForeColor = Color.FromArgb(52, 73, 94),
                    AutoSize = true,
                    Location = new Point(28, maxHeight - barHeight - 18)
                };
                barContainer.Controls.Add(lblMinutes);

                Label lblDate = new Label
                {
                    Text = date.ToString("dd/MM"),
                    Font = new Font("Segoe UI", 8),
                    ForeColor = Color.FromArgb(127, 140, 141),
                    AutoSize = true,
                    Location = new Point(20, maxHeight + 5)
                };
                barContainer.Controls.Add(lblDate);

                pnlWeeklyChart.Controls.Add(barContainer);
            }
        }

        private void BtnExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                var reportService = new WindowsFormsApp1.Services.ReportService();
                
                // Collect all statistics
                var streak = DataManager.Instance.GetReadingStreak(userId);
                int todayMinutes = DataManager.Instance.GetTodayReadingMinutes(userId);
                int monthlyBooks = DataManager.Instance.GetMonthlyBooksRead(userId);
                int yearlyBooks = DataManager.Instance.GetYearlyBooksRead(userId);
                var weeklyStats = DataManager.Instance.GetWeeklyStats(userId);
                var goals = DataManager.Instance.GetActiveGoals(userId);

                // Create report
                reportService.CreateGoalsReport(userId, streak, todayMinutes, monthlyBooks, yearlyBooks, weeklyStats, goals);
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i xu?t báo cáo: " + ex.Message, "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetDailyGoal_Click(object sender, EventArgs e)
        {
            // T?o overlay m?
            Form overlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
                Size = this.Size,
                BackColor = Color.Black,
                Opacity = 0.6,
                ShowInTaskbar = false,
                TopMost = false
            };
            
            overlay.Show(this);
            
            using (var form = new GoalInputForm("DAILY_MINUTES", "Ð?t m?c tiêu ð?c hàng ngày (phút):", 30, 10, 300))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                var result = form.ShowDialog(this);
                
                overlay.Close();
                overlay.Dispose();
                
                if (result == DialogResult.OK)
                {
                    var oldGoals = DataManager.Instance.GetActiveGoals(userId)
                        .Where(g => g.GoalType == "DAILY_MINUTES").ToList();
                    foreach (var g in oldGoals)
                    {
                        DataManager.Instance.UpdateGoal(g.Id, false);
                    }

                    DataManager.Instance.CreateReadingGoal(userId, "DAILY_MINUTES", form.TargetValue);
                    MessageBox.Show($"Ð? ð?t m?c tiêu ð?c {form.TargetValue} phút m?i ngày!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStatistics();
                }
            }
        }

        private void BtnSetMonthlyGoal_Click(object sender, EventArgs e)
        {
            // T?o overlay m?
            Form overlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
                Size = this.Size,
                BackColor = Color.Black,
                Opacity = 0.6,
                ShowInTaskbar = false,
                TopMost = false
            };
            
            overlay.Show(this);
            
            using (var form = new GoalInputForm("MONTHLY_BOOKS", "Ð?t m?c tiêu ð?c trong tháng (cu?n):", 3, 1, 30))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                var result = form.ShowDialog(this);
                
                overlay.Close();
                overlay.Dispose();
                
                if (result == DialogResult.OK)
                {
                    var oldGoals = DataManager.Instance.GetActiveGoals(userId)
                        .Where(g => g.GoalType == "MONTHLY_BOOKS").ToList();
                    foreach (var g in oldGoals)
                    {
                        DataManager.Instance.UpdateGoal(g.Id, false);
                    }

                    DataManager.Instance.CreateReadingGoal(userId, "MONTHLY_BOOKS", form.TargetValue);
                    MessageBox.Show($"Ð? ð?t m?c tiêu ð?c {form.TargetValue} cu?n m?i tháng!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStatistics();
                }
            }
        }

        private void BtnSetYearlyGoal_Click(object sender, EventArgs e)
        {
            // T?o overlay m?
            Form overlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
                Size = this.Size,
                BackColor = Color.Black,
                Opacity = 0.6,
                ShowInTaskbar = false,
                TopMost = false
            };
            
            overlay.Show(this);
            
            using (var form = new GoalInputForm("YEARLY_BOOKS", "Ð?t m?c tiêu ð?c trong nãm (cu?n):", 12, 1, 100))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                var result = form.ShowDialog(this);
                
                overlay.Close();
                overlay.Dispose();
                
                if (result == DialogResult.OK)
                {
                    var oldGoals = DataManager.Instance.GetActiveGoals(userId)
                        .Where(g => g.GoalType == "YEARLY_BOOKS").ToList();
                    foreach (var g in oldGoals)
                    {
                        DataManager.Instance.UpdateGoal(g.Id, false);
                    }

                    DataManager.Instance.CreateReadingGoal(userId, "YEARLY_BOOKS", form.TargetValue);
                    MessageBox.Show($"Ð? ð?t m?c tiêu ð?c {form.TargetValue} cu?n trong nãm!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStatistics();
                }
            }
        }
        
        // ================= [CLASS HELPER: GoalInputForm] =================
    }

    public class GoalInputForm : Form
    {
        public int TargetValue { get; private set; }
        
        private NumericUpDown numValue;
        private string _goalType;

        public GoalInputForm(string goalType, string prompt, int defaultValue, int minValue, int maxValue)
        {
            _goalType = goalType;
            
            this.Text = "Ð?t m?c tiêu";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None; // Lo?i b? vi?n ð? t?o hi?u ?ng hi?n ð?i
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.Opacity = 0.98; // Ð? trong su?t nh?
            
            // T?o shadow effect
            this.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(100, 100, 100), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
            
            // Xác ð?nh màu và text theo lo?i m?c tiêu
            Color accentColor;
            string iconLabel;
            string titleText;
            
            switch (goalType)
            {
                case "DAILY_MINUTES":
                    accentColor = Color.FromArgb(46, 204, 113);
                    iconLabel = "NGÀY";
                    titleText = "M?c tiêu hàng ngày";
                    break;
                case "MONTHLY_BOOKS":
                    accentColor = Color.FromArgb(241, 196, 15);
                    iconLabel = "THÁNG";
                    titleText = "M?c tiêu tháng này";
                    break;
                case "YEARLY_BOOKS":
                    accentColor = Color.FromArgb(155, 89, 182);
                    iconLabel = "NÃM";
                    titleText = "M?c tiêu nãm này";
                    break;
                default:
                    accentColor = Color.FromArgb(138, 180, 248);
                    iconLabel = "M?C TIÊU";
                    titleText = "M?c tiêu ð?c sách";
                    break;
            }
            
            // Close button (X) ? góc ph?i trên
            Button btnCloseX = new Button
            {
                Text = "?",
                Size = new Size(35, 35),
                Location = new Point(this.Width - 40, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCloseX.FlatAppearance.BorderSize = 0;
            btnCloseX.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnCloseX.MouseEnter += (s, e) => { btnCloseX.ForeColor = Color.Red; };
            btnCloseX.MouseLeave += (s, e) => { btnCloseX.ForeColor = Color.Gray; };
            
            // Icon Label v?i background màu (thay emoji)
            Panel pnlIconBg = new Panel
            {
                Size = new Size(70, 70),
                Location = new Point(30, 40),
                BackColor = accentColor
            };
            
            Label lblIcon = new Label
            {
                Text = iconLabel,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            pnlIconBg.Controls.Add(lblIcon);
            
            // Title
            Label lblTitle = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(115, 50),
                Size = new Size(350, 50),
                BackColor = Color.Transparent
            };
            
            // Description
            Label lblDescription = new Label
            {
                Text = prompt,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(30, 130),
                Size = new Size(440, 30),
                BackColor = Color.Transparent
            };
            
            // Label "Giá tr? m?c tiêu:"
            Label lblValueLabel = new Label
            {
                Text = "Giá tr? m?c tiêu:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(30, 175),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            
            // NumericUpDown v?i style ð?p hõn
            numValue = new NumericUpDown
            {
                Location = new Point(220, 172),
                Size = new Size(250, 35),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Minimum = minValue,
                Maximum = maxValue,
                Value = defaultValue,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = accentColor,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };
            
            // Buttons
            Button btnOK = new Button
            {
                Text = "? LÝU M?C TIÊU",
                Location = new Point(280, 230),
                Size = new Size(190, 50),
                BackColor = accentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) => 
            {
                TargetValue = (int)numValue.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            
            Button btnCancel = new Button
            {
                Text = "H?y",
                Location = new Point(170, 230),
                Size = new Size(100, 50),
                BackColor = Color.Transparent,
                ForeColor = Color.Silver,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.BorderColor = Color.Gray;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            
            this.Controls.Add(btnCloseX);
            this.Controls.Add(pnlIconBg);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDescription);
            this.Controls.Add(lblValueLabel);
            this.Controls.Add(numValue);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            
            // Focus vào NumericUpDown khi m?
            this.Shown += (s, e) => numValue.Focus();
        }
    }
}
