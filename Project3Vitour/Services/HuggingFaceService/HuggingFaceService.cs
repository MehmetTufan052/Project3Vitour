using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Project3Vitour.Services.HuggingFaceService
{
    public class HuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelUrl;

        public HuggingFaceService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey =
                config["HuggingFace:ApiKey"] ??
                config["HUGGINGFACE_API_KEY"] ??
                config["HF_API_KEY"] ??
                string.Empty;

            var modelId = config["HuggingFace:ModelId"] ?? "savasy/bert-base-turkish-sentiment-cased";
            _modelUrl = $"https://router.huggingface.co/hf-inference/models/{modelId}";
        }

        public async Task<(string label, double score, bool success)> AnalyzeSentimentAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return ("Nötr", 0, false);

            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                    return ("Nötr", 0, false);

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);

                var payload = new
                {
                    inputs = text,
                    options = new { wait_for_model = true }
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_modelUrl, content);
                if (!response.IsSuccessStatusCode)
                    return ("Nötr", 0, false);

                var responseString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseString))
                    return ("Nötr", 0, false);

                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("error", out _))
                        return ("Nötr", 0, false);

                    return ("Nötr", 0, false);
                }

                JsonElement candidates;
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var first = root[0];
                    candidates = first.ValueKind == JsonValueKind.Array ? first : root;
                }
                else
                {
                    return ("Nötr", 0, false);
                }

                var best = candidates
                    .EnumerateArray()
                    .Where(x => x.ValueKind == JsonValueKind.Object && x.TryGetProperty("score", out _))
                    .OrderByDescending(x => x.GetProperty("score").GetDouble())
                    .FirstOrDefault();

                if (best.ValueKind != JsonValueKind.Object)
                    return ("Nötr", 0, false);

                var rawLabel = best.TryGetProperty("label", out var labelProp)
                    ? (labelProp.GetString() ?? string.Empty)
                    : string.Empty;

                var score = best.TryGetProperty("score", out var scoreProp)
                    ? scoreProp.GetDouble()
                    : 0;

                var label = NormalizeApiLabel(rawLabel);
                return (label, Math.Round(score, 4), true);
            }
            catch
            {
                return ("Nötr", 0, false);
            }
        }

        private static string NormalizeApiLabel(string rawLabel)
        {
            var normalized = (rawLabel ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return "Nötr";
            }

            if (normalized == "label_1") return "Olumlu";
            if (normalized == "label_0") return "Olumsuz";
            if (normalized == "label_2") return "Nötr";

            if (normalized.Contains("positive") || normalized.Contains("olumlu") || normalized == "pos")
            {
                return "Olumlu";
            }

            if (normalized.Contains("negative") || normalized.Contains("olumsuz") || normalized == "neg")
            {
                return "Olumsuz";
            }

            if (normalized.Contains("neutral") || normalized.Contains("notr") || normalized.Contains("nötr"))
            {
                return "Nötr";
            }

            return "Nötr";
        }
    }
}
