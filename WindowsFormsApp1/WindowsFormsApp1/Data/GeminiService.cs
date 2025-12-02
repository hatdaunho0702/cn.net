using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp1.Data
{
    public class GeminiService
    {
        // ⚠️ Thay API Key MỚI của bạn vào đây (Key cũ đã bị lộ, hãy xóa đi)
        private const string API_KEY = "AIzaSyDsJXnCt0OMttowASMb7gqtX3eRr-34uNc";

        // SỬA LỖI Ở ĐÂY: Dùng v1beta thay vì v1
        private const string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";



        public async Task<string> AskGemini(string contextText, string userPrompt)
        {
            try
            {
                var options = new RestClientOptions(API_URL);
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.Method = Method.Post;

                request.AddQueryParameter("key", API_KEY);
                request.AddHeader("Content-Type", "application/json");

                // [SỬA ĐỔI TẠI ĐÂY] Tăng giới hạn từ 30,000 lên 2,000,000 ký tự (đủ cho sách dày)
                // Gemini Flash hỗ trợ context rất lớn nên không cần cắt quá ngắn.
                string safeContext = contextText.Length > 2000000 ? contextText.Substring(0, 2000000) : contextText;

                string finalPrompt = $"Dựa vào nội dung sách sau:\n---\n{safeContext}\n---\nHãy trả lời câu hỏi: {userPrompt}";

                var body = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text = finalPrompt }
                    }
                }
            }
                };

                request.AddJsonBody(body);
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var jsonResponse = JObject.Parse(response.Content);
                    string aiReply = jsonResponse["candidates"]?[0]?["content"]?["sparts"]?[0]?["text"]?.ToString();
                    return aiReply ?? "AI không trả lời (null).";
                }
                else
                {
                    return $"Lỗi API ({response.StatusCode}): {response.Content}";
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi hệ thống: {ex.Message}";
            }
        }
    }
}