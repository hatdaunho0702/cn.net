using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.Data; // Đảm bảo namespace đúng

namespace WindowsFormsApp1.Services // Hoặc WindowsFormsApp1.Data
{
    public class ReportService
    {
        // Hàm tạo file HTML tạm và mở Form in ấn
        private void PrintHtml(string htmlContent, string documentTitle)
        {
            try
            {
                // 1. Tạo file HTML tạm trong thư mục Temp của Windows
                string tempPath = Path.Combine(Path.GetTempPath(), $"Report_{Guid.NewGuid()}.html");
                File.WriteAllText(tempPath, htmlContent, Encoding.UTF8);

                // 2. Tạo một Form mới chứa WebBrowser để hiển thị và in
                Form printForm = new Form
                {
                    Text = "Xem trước bản in - " + documentTitle,
                    Size = new System.Drawing.Size(850, 800),
                    StartPosition = FormStartPosition.CenterScreen
                };

                WebBrowser browser = new WebBrowser { Dock = DockStyle.Fill };
                browser.Navigate(tempPath);

                // Thêm nút In ở trên cùng
                Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 40 };
                Button btnPrint = new Button { Text = "🖨 IN BÁO CÁO", Left = 10, Top = 5, Width = 120, Height = 30, BackColor = System.Drawing.Color.DodgerBlue, ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };

                btnPrint.Click += (s, e) => { browser.ShowPrintDialog(); };

                topPanel.Controls.Add(btnPrint);
                printForm.Controls.Add(browser);
                printForm.Controls.Add(topPanel);

                printForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo báo cáo: " + ex.Message);
            }
        }

        // --- BÁO CÁO 1: DANH SÁCH SÁCH ---
        public void CreateBookListReport(List<Book> books, string userName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><style>");
            sb.Append("body { font-family: 'Segoe UI', Arial; padding: 20px; }");
            sb.Append("h1 { text-align: center; color: #333; }");
            sb.Append("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            sb.Append("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            sb.Append("th { background-color: #f2f2f2; }");
            sb.Append(".footer { margin-top: 30px; text-align: right; font-style: italic; }");
            sb.Append("</style></head><body>");

            sb.Append($"<h1>BÁO CÁO THỐNG KÊ SÁCH</h1>");
            sb.Append($"<p><strong>Người xuất:</strong> {userName}</p>");
            sb.Append($"<p><strong>Ngày xuất:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.Append($"<p><strong>Tổng số sách:</strong> {books.Count}</p>");

            sb.Append("<table>");
            sb.Append("<thead><tr><th>STT</th><th>Tên Sách</th><th>Tác Giả</th><th>Ngày Thêm</th><th>Tiến Độ</th></tr></thead>");
            sb.Append("<tbody>");

            int stt = 1;
            foreach (var b in books)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{stt++}</td>");
                sb.Append($"<td>{b.Title}</td>");
                sb.Append($"<td>{b.Author}</td>");
                sb.Append($"<td>{b.DateAdded:dd/MM/yyyy}</td>");
                sb.Append($"<td>{b.Progress:F1}%</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
            sb.Append($"<div class='footer'><p>Koodo Reader Report System</p></div>");
            sb.Append("</body></html>");

            PrintHtml(sb.ToString(), "DanhSachSach");
        }

        // --- BÁO CÁO 2: GHI CHÚ CỦA SÁCH ---
        public void CreateNotesReport(Book book, List<Highlight> highlights)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><style>");
            sb.Append("body { font-family: 'Segoe UI', Arial; padding: 20px; }");
            sb.Append("h1 { color: #0078D7; border-bottom: 2px solid #0078D7; padding-bottom: 10px; }");
            sb.Append(".book-info { background: #f9f9f9; padding: 15px; border-radius: 5px; margin-bottom: 20px; }");
            sb.Append(".note-item { border-left: 4px solid; margin-bottom: 20px; padding-left: 15px; }");
            sb.Append(".quote { font-style: italic; color: #555; background: #fffde7; padding: 5px; }");
            sb.Append(".user-note { font-weight: bold; margin-top: 5px; color: #333; }");
            sb.Append(".meta { font-size: 0.85em; color: #999; }");
            sb.Append("</style></head><body>");

            sb.Append($"<h1>TỔNG HỢP GHI CHÚ</h1>");

            sb.Append($"<div class='book-info'>");
            sb.Append($"<h2>📖 {book.Title}</h2>");
            sb.Append($"<p>Tác giả: {book.Author}</p>");
            sb.Append($"<p>Số lượng ghi chú: {highlights.Count}</p>");
            sb.Append($"</div>");

            foreach (var hl in highlights)
            {
                string color = hl.ColorHex ?? "#FFD700";
                sb.Append($"<div class='note-item' style='border-color:{color}'>");
                sb.Append($"<div class='quote'>\"{hl.SelectedText}\"</div>");

                if (!string.IsNullOrEmpty(hl.Note))
                {
                    sb.Append($"<div class='user-note'>📝 {hl.Note}</div>");
                }

                sb.Append($"<div class='meta'>Chương {hl.ChapterIndex + 1} - Ngày tạo: {hl.DateCreated:dd/MM/yyyy}</div>");
                sb.Append("</div>");
            }

            sb.Append("</body></html>");

            PrintHtml(sb.ToString(), $"GhiChu_{book.Title}");
        }

        // --- BÁO CÁO 3: THỐNG KÊ MỤC TIÊU ĐỌC SÁCH ---
        public void CreateGoalsReport(int userId, ReadingStreak streak, int todayMinutes, 
            int monthlyBooks, int yearlyBooks, List<DailyReadingStats> weeklyStats, List<ReadingGoal> goals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><style>");
            sb.Append("body { font-family: 'Segoe UI', Arial; padding: 30px; background: #f5f6fa; }");
            sb.Append("h1 { color: #2c3e50; text-align: center; margin-bottom: 30px; }");
            sb.Append(".container { max-width: 800px; margin: 0 auto; }");
            sb.Append(".card { background: white; border-radius: 8px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            sb.Append(".card h2 { color: #34495e; margin-top: 0; border-left: 4px solid; padding-left: 15px; }");
            sb.Append(".stat-row { display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #ecf0f1; }");
            sb.Append(".stat-label { font-weight: 600; color: #7f8c8d; }");
            sb.Append(".stat-value { font-weight: bold; color: #2c3e50; font-size: 1.1em; }");
            sb.Append(".progress-bar { height: 20px; background: #ecf0f1; border-radius: 10px; overflow: hidden; margin: 10px 0; }");
            sb.Append(".progress-fill { height: 100%; background: linear-gradient(90deg, #3498db, #2ecc71); transition: width 0.3s; }");
            sb.Append(".streak-big { font-size: 48px; font-weight: bold; color: #e74c3c; text-align: center; margin: 20px 0; }");
            sb.Append(".chart { display: flex; align-items: flex-end; justify-content: space-around; height: 150px; margin: 20px 0; }");
            sb.Append(".bar { width: 60px; background: linear-gradient(180deg, #3498db, #2980b9); border-radius: 4px 4px 0 0; text-align: center; color: white; font-size: 12px; padding-top: 5px; }");
            sb.Append(".bar-label { text-align: center; font-size: 11px; color: #7f8c8d; margin-top: 5px; }");
            sb.Append(".goal-item { background: #ecf0f1; padding: 12px; border-radius: 6px; margin: 10px 0; }");
            sb.Append(".footer { text-align: center; color: #95a5a6; margin-top: 30px; font-size: 0.9em; }");
            sb.Append("</style></head><body>");

            sb.Append("<div class='container'>");
            sb.Append("<h1>📊 BAO CAO THONG KE DOC SACH</h1>");
            
            var user = DataManager.Instance.GetUserById(userId);
            sb.Append($"<p style='text-align:center; color:#7f8c8d;'>Nguoi dung: <strong>{user?.DisplayName ?? "Unknown"}</strong> | Ngay xuat: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");

            // STREAK CARD
            sb.Append("<div class='card'>");
            sb.Append("<h2 style='border-color:#e74c3c;'>🔥 CHUOI NGAY DOC LIEN TUC</h2>");
            sb.Append($"<div class='streak-big'>{streak.CurrentStreak} NGAY</div>");
            sb.Append("<div class='stat-row'>");
            sb.Append("<span class='stat-label'>Ky luc ca nhan:</span>");
            sb.Append($"<span class='stat-value'>{streak.LongestStreak} ngay</span>");
            sb.Append("</div>");
            sb.Append("<div class='stat-row' style='border:none;'>");
            sb.Append("<span class='stat-label'>Lan doc gan nhat:</span>");
            sb.Append($"<span class='stat-value'>{streak.LastReadDate:dd/MM/yyyy}</span>");
            sb.Append("</div>");
            sb.Append("</div>");

            // DAILY GOAL
            var dailyGoal = goals.FirstOrDefault(g => g.GoalType == "DAILY_MINUTES");
            int dailyTarget = dailyGoal?.TargetValue ?? 30;
            int dailyProgress = Math.Min(100, (int)((double)todayMinutes / dailyTarget * 100));
            
            sb.Append("<div class='card'>");
            sb.Append("<h2 style='border-color:#2ecc71;'>🎯 MUC TIEU HOM NAY</h2>");
            sb.Append("<div class='stat-row'>");
            sb.Append("<span class='stat-label'>Thoi gian doc:</span>");
            sb.Append($"<span class='stat-value'>{todayMinutes} / {dailyTarget} phut</span>");
            sb.Append("</div>");
            sb.Append($"<div class='progress-bar'><div class='progress-fill' style='width:{dailyProgress}%;'></div></div>");
            sb.Append($"<p style='text-align:center; color:#7f8c8d; margin:0;'>Hoan thanh: {dailyProgress}%</p>");
            sb.Append("</div>");

            // MONTHLY GOAL
            var monthlyGoal = goals.FirstOrDefault(g => g.GoalType == "MONTHLY_BOOKS");
            int monthlyTarget = monthlyGoal?.TargetValue ?? 3;
            int monthlyProgress = Math.Min(100, (int)((double)monthlyBooks / monthlyTarget * 100));
            
            sb.Append("<div class='card'>");
            sb.Append("<h2 style='border-color:#f1c40f;'>📅 MUC TIEU THANG NAY</h2>");
            sb.Append("<div class='stat-row'>");
            sb.Append("<span class='stat-label'>So sach da doc:</span>");
            sb.Append($"<span class='stat-value'>{monthlyBooks} / {monthlyTarget} cuon</span>");
            sb.Append("</div>");
            sb.Append($"<div class='progress-bar'><div class='progress-fill' style='width:{monthlyProgress}%; background:linear-gradient(90deg, #f39c12, #f1c40f);'></div></div>");
            sb.Append($"<p style='text-align:center; color:#7f8c8d; margin:0;'>Hoan thanh: {monthlyProgress}%</p>");
            sb.Append("</div>");

            // YEARLY GOAL
            var yearlyGoal = goals.FirstOrDefault(g => g.GoalType == "YEARLY_BOOKS");
            int yearlyTarget = yearlyGoal?.TargetValue ?? 12;
            int yearlyProgress = Math.Min(100, (int)((double)yearlyBooks / yearlyTarget * 100));
            
            sb.Append("<div class='card'>");
            sb.Append("<h2 style='border-color:#9b59b6;'>📚 MUC TIEU NAM NAY</h2>");
            sb.Append("<div class='stat-row'>");
            sb.Append("<span class='stat-label'>So sach da doc:</span>");
            sb.Append($"<span class='stat-value'>{yearlyBooks} / {yearlyTarget} cuon</span>");
            sb.Append("</div>");
            sb.Append($"<div class='progress-bar'><div class='progress-fill' style='width:{yearlyProgress}%; background:linear-gradient(90deg, #8e44ad, #9b59b6);'></div></div>");
            sb.Append($"<p style='text-align:center; color:#7f8c8d; margin:0;'>Hoan thanh: {yearlyProgress}%</p>");
            sb.Append("</div>");

            // WEEKLY CHART
            sb.Append("<div class='card'>");
            sb.Append("<h2 style='border-color:#3498db;'>📈 THONG KE 7 NGAY GAN NHAT</h2>");
            sb.Append("<div class='chart'>");
            
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var stat = weeklyStats.FirstOrDefault(s => s.Date.Date == date.Date);
                int minutes = stat?.TotalMinutes ?? 0;
                int height = Math.Min(120, (int)((double)minutes / 60 * 120));
                string color = minutes >= dailyTarget ? "#2ecc71" : "#95a5a6";
                
                sb.Append("<div>");
                sb.Append($"<div class='bar' style='height:{height}px; background:linear-gradient(180deg, {color}, {color});'>{minutes}p</div>");
                sb.Append($"<div class='bar-label'>{date:dd/MM}</div>");
                sb.Append("</div>");
            }
            
            sb.Append("</div>");
            
            int totalWeekMinutes = weeklyStats.Sum(s => s.TotalMinutes);
            int avgWeekMinutes = weeklyStats.Count > 0 ? totalWeekMinutes / 7 : 0;
            
            sb.Append("<div class='stat-row'>");
            sb.Append("<span class='stat-label'>Tong thoi gian:</span>");
            sb.Append($"<span class='stat-value'>{totalWeekMinutes} phut</span>");
            sb.Append("</div>");
            sb.Append("<div class='stat-row' style='border:none;'>");
            sb.Append("<span class='stat-label'>Trung binh moi ngay:</span>");
            sb.Append($"<span class='stat-value'>{avgWeekMinutes} phut</span>");
            sb.Append("</div>");
            sb.Append("</div>");

            // ACTIVE GOALS
            sb.Append("<div class='card'>");
            sb.Append("<h2 style='border-color:#34495e;'>🎯 CAC MUC TIEU DANG HOAT DONG</h2>");
            
            if (goals.Count == 0)
            {
                sb.Append("<p style='text-align:center; color:#95a5a6;'>Chua co muc tieu nao duoc dat.</p>");
            }
            else
            {
                foreach (var goal in goals)
                {
                    string goalName = goal.GoalType == "DAILY_MINUTES" ? "Doc hang ngay" :
                                     goal.GoalType == "MONTHLY_BOOKS" ? "Doc hang thang" : "Doc hang nam";
                    string unit = goal.GoalType == "DAILY_MINUTES" ? "phut" : "cuon";
                    
                    sb.Append("<div class='goal-item'>");
                    sb.Append($"<strong>{goalName}:</strong> {goal.TargetValue} {unit} | ");
                    sb.Append($"Bat dau: {goal.StartDate:dd/MM/yyyy}");
                    sb.Append("</div>");
                }
            }
            
            sb.Append("</div>");

            sb.Append("<div class='footer'>");
            sb.Append("<p>Bao cao nay duoc tao tu he thong Koodo Reader</p>");
            sb.Append($"<p>Tiep tuc phan dau de dat duoc muc tieu!</p>");
            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</body></html>");

            PrintHtml(sb.ToString(), $"BaoCaoMucTieu_{DateTime.Now:yyyyMMdd}");
        }

        // --- BÁO CÁO 4: TỔNG HỢP GHI CHÚ & ĐÁNH DẤU ---
        public void CreateNotesHighlightsReport(int userId, List<Highlight> highlights, List<Highlight> notes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head><style>");
            sb.Append("body { font-family: 'Segoe UI', Arial; padding: 30px; background: #f5f6fa; }");
            sb.Append("h1 { color: #2c3e50; text-align: center; margin-bottom: 20px; }");
            sb.Append(".container { max-width: 900px; margin: 0 auto; }");
            sb.Append(".stats { display: flex; justify-content: space-around; margin: 30px 0; }");
            sb.Append(".stat-box { background: white; padding: 20px; border-radius: 10px; text-align: center; box-shadow: 0 2px 4px rgba(0,0,0,0.1); flex: 1; margin: 0 10px; }");
            sb.Append(".stat-number { font-size: 48px; font-weight: bold; color: #3498db; }");
            sb.Append(".stat-label { color: #7f8c8d; margin-top: 10px; }");
            sb.Append(".section { background: white; padding: 25px; border-radius: 10px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            sb.Append(".section-title { color: #34495e; font-size: 20px; font-weight: bold; margin-bottom: 15px; border-left: 4px solid; padding-left: 15px; }");
            sb.Append(".item { border-left: 5px solid; padding: 15px; margin: 15px 0; background: #f9f9f9; border-radius: 5px; }");
            sb.Append(".book-title { font-weight: bold; color: #2c3e50; margin-bottom: 8px; }");
            sb.Append(".quote { font-style: italic; color: #555; background: #fffde7; padding: 8px; margin: 8px 0; border-radius: 3px; }");
            sb.Append(".note { font-weight: bold; color: #e67e22; margin-top: 8px; }");
            sb.Append(".meta { font-size: 12px; color: #95a5a6; margin-top: 5px; }");
            sb.Append(".book-group { margin-bottom: 30px; }");
            sb.Append(".book-group-title { background: #3498db; color: white; padding: 12px 20px; border-radius: 5px; font-size: 16px; font-weight: bold; margin-bottom: 15px; }");
            sb.Append(".footer { text-align: center; color: #95a5a6; margin-top: 40px; padding: 20px; }");
            sb.Append("</style></head><body>");

            sb.Append("<div class='container'>");
            
            var user = DataManager.Instance.GetUserById(userId);
            sb.Append($"<h1>📊 BÁO CÁO GHI CHÚ & ĐÁNH DẤU</h1>");
            sb.Append($"<p style='text-align:center; color:#7f8c8d;'>Người dùng: <strong>{user?.DisplayName ?? "Unknown"}</strong> | Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");

            // STATISTICS
            sb.Append("<div class='stats'>");
            
            sb.Append("<div class='stat-box'>");
            sb.Append($"<div class='stat-number'>{highlights.Count}</div>");
            sb.Append("<div class='stat-label'>⭐ Đánh dấu</div>");
            sb.Append("</div>");
            
            sb.Append("<div class='stat-box'>");
            sb.Append($"<div class='stat-number'>{notes.Count}</div>");
            sb.Append("<div class='stat-label'>📝 Ghi chú</div>");
            sb.Append("</div>");
            
            int totalBooks = highlights.Concat(notes).Select(h => h.BookId).Distinct().Count();
            sb.Append("<div class='stat-box'>");
            sb.Append($"<div class='stat-number'>{totalBooks}</div>");
            sb.Append("<div class='stat-label'>📚 Sách đã đánh dấu</div>");
            sb.Append("</div>");
            
            sb.Append("</div>");

            // GROUP BY BOOK - HIGHLIGHTS
            if (highlights.Count > 0)
            {
                sb.Append("<div class='section'>");
                sb.Append("<div class='section-title' style='border-color:#f39c12;'>⭐ DANH SÁCH ĐÁNH DẤU</div>");
                
                var highlightGroups = highlights.GroupBy(h => h.BookTitle).OrderByDescending(g => g.Count());
                
                foreach (var group in highlightGroups)
                {
                    sb.Append("<div class='book-group'>");
                    sb.Append($"<div class='book-group-title'>📖 {group.Key} ({group.Count()} đánh dấu)</div>");
                    
                    foreach (var hl in group)
                    {
                        string color = hl.ColorHex ?? "#FFD700";
                        sb.Append($"<div class='item' style='border-color:{color};'>");
                        sb.Append($"<div class='quote'>\"{hl.SelectedText}\"</div>");
                        sb.Append($"<div class='meta'>Chương {hl.ChapterIndex + 1} • {hl.DateCreated:dd/MM/yyyy HH:mm}</div>");
                        sb.Append("</div>");
                    }
                    
                    sb.Append("</div>");
                }
                
                sb.Append("</div>");
            }

            // GROUP BY BOOK - NOTES
            if (notes.Count > 0)
            {
                sb.Append("<div class='section'>");
                sb.Append("<div class='section-title' style='border-color:#e74c3c;'>📝 DANH SÁCH GHI CHÚ</div>");
                
                var noteGroups = notes.GroupBy(n => n.BookTitle).OrderByDescending(g => g.Count());
                
                foreach (var group in noteGroups)
                {
                    sb.Append("<div class='book-group'>");
                    sb.Append($"<div class='book-group-title'>📖 {group.Key} ({group.Count()} ghi chú)</div>");
                    
                    foreach (var note in group)
                    {
                        string color = note.ColorHex ?? "#FFD700";
                        sb.Append($"<div class='item' style='border-color:{color};'>");
                        sb.Append($"<div class='quote'>\"{note.SelectedText}\"</div>");
                        
                        if (!string.IsNullOrEmpty(note.Note))
                        {
                            sb.Append($"<div class='note'>📝 {note.Note}</div>");
                        }
                        
                        sb.Append($"<div class='meta'>Chương {note.ChapterIndex + 1} • {note.DateCreated:dd/MM/yyyy HH:mm}</div>");
                        sb.Append("</div>");
                    }
                    
                    sb.Append("</div>");
                }
                
                sb.Append("</div>");
            }

            // TOP BOOKS WITH MOST NOTES
            sb.Append("<div class='section'>");
            sb.Append("<div class='section-title' style='border-color:#9b59b6;'>📊 THỐNG KÊ THEO SÁCH</div>");
            
            var bookStats = highlights.Concat(notes)
                .GroupBy(h => h.BookTitle)
                .Select(g => new { BookTitle = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10);
            
            sb.Append("<table style='width:100%; border-collapse:collapse;'>");
            sb.Append("<thead><tr style='background:#ecf0f1;'><th style='padding:10px; text-align:left;'>Sách</th><th style='padding:10px; text-align:center;'>Số lượng</th></tr></thead>");
            sb.Append("<tbody>");
            
            int rank = 1;
            foreach (var stat in bookStats)
            {
                sb.Append("<tr style='border-bottom:1px solid #ecf0f1;'>");
                sb.Append($"<td style='padding:10px;'>{rank}. {stat.BookTitle}</td>");
                sb.Append($"<td style='padding:10px; text-align:center; font-weight:bold; color:#3498db;'>{stat.Count}</td>");
                sb.Append("</tr>");
                rank++;
            }
            
            sb.Append("</tbody></table>");
            sb.Append("</div>");

            sb.Append("<div class='footer'>");
            sb.Append("<p>Báo cáo này được tạo từ hệ thống Koodo Reader</p>");
            sb.Append($"<p>Tiếp tục đọc và ghi chú những điều hay ho!</p>");
            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</body></html>");

            PrintHtml(sb.ToString(), $"BaoCaoGhiChu_{DateTime.Now:yyyyMMdd}");
        }
    }
}