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
            if (string.IsNullOrWhiteSpace(text))
                return ("Nötr", 0);

            try
            {
                if (!string.IsNullOrWhiteSpace(_apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _apiKey);
                }

                var payload = new { inputs = text };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ModelUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseString))
                    return ("Nötr", 0);

                var trimmed = responseString.TrimStart();
                if (!(trimmed.StartsWith("[") || trimmed.StartsWith("{")))
                    return ("Nötr", 0);

                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("error", out _))
                        return ("Nötr", 0);
                    return ("Nötr", 0);
                }

                JsonElement candidates;
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var first = root[0];
                    candidates = first.ValueKind == JsonValueKind.Array ? first : root;
                }
                else
                {
                    return ("Nötr", 0);
                }

                var best = candidates
                    .EnumerateArray()
                    .Where(x => x.ValueKind == JsonValueKind.Object && x.TryGetProperty("score", out _))
                    .OrderByDescending(x => x.GetProperty("score").GetDouble())
                    .FirstOrDefault();

                if (best.ValueKind != JsonValueKind.Object)
                    return ("Nötr", 0);

                var rawLabel = best.TryGetProperty("label", out var labelProp)
                    ? (labelProp.GetString() ?? string.Empty)
                    : string.Empty;
                var score = best.TryGetProperty("score", out var scoreProp)
                    ? scoreProp.GetDouble()
                    : 0;

                var label = rawLabel.ToLowerInvariant() switch
                {
                    "positive" => "Olumlu",
                    "negative" => "Olumsuz",
                    _ => "Nötr"
                };

                return (label, Math.Round(score, 4));
            }
            catch
            {
                return ("Nötr", 0);
            }
        }
    }
}
