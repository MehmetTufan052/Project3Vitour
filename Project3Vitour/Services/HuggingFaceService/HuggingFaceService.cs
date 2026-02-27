using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Project3Vitour.Services.HuggingFaceService
{
    public class HuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ModelUrl =
            "https://api-inference.huggingface.co/models/savasy/bert-base-turkish-sentiment-cased";

        public HuggingFaceService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["HuggingFace:ApiKey"];
        }

        public async Task<(string label, double score)> AnalyzeSentimentAsync(string text)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var payload = new { inputs = text };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ModelUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Model [[{label, score}, {label, score}]] formatında döner
            using var doc = JsonDocument.Parse(responseString);
            var results = doc.RootElement[0].EnumerateArray()
                            .OrderByDescending(x => x.GetProperty("score").GetDouble())
                            .First();

            var rawLabel = results.GetProperty("label").GetString();
            var score = results.GetProperty("score").GetDouble();

            // Türkçeleştir
            var label = rawLabel switch
            {
                "positive" => "Olumlu",
                "negative" => "Olumsuz",
                _ => "Nötr"
            };

            return (label, Math.Round(score, 4));
        }
    }
}

