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
    }
}