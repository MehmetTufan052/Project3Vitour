using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Project3Vitour.Services.HuggingFaceService
{
    public class HuggingFaceService
    {
        private const double NeutralConfidenceThreshold = 0.60;
        private const double NeutralMarginThreshold = 0.12;
        private static readonly string[] NeutralCuePhrases =
        {
            "genel olarak",
            "standart bir deneyim",
            "standart düzeyde",
            "ortalama bir deneyim",
            "ortalama düzeyde",
            "dengeli bir deneyim",
            "normal seviyede",
            "beklentimi normal",
            "beklentimi karşıladı",
            "çok güçlü olumlu ya da olumsuz",
            "güçlü olumlu ya da olumsuz",
            "ne çok etkileyici ne de kötü",
            "ne çok iyi ne de kötü",
            "ne iyi ne kötü",
            "orta seviyede",
            "olması gerektiği gibiydi",
            "belirgin bir aksaklık yaşanmadı",
            "ciddi bir problem yaşanmadı"
        };

        private static readonly string[] StrongPositivePhrases =
        {
            "harika",
            "mükemmel",
            "muhteşem",
            "çok memnun",
            "inanılmaz",
            "mükemmeldi",
            "şahane",
            "çok güzeldi"
        };

        private static readonly string[] StrongNegativePhrases =
        {
            "berbat",
            "kötüydü",
            "hayal kırıklığı",
            "memnun kalmadım",
            "rezaletti",
            "yetersizdi",
            "çok kötü",
            "pişman oldum"
        };

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

                var ranked = candidates
                    .EnumerateArray()
                    .Where(x => x.ValueKind == JsonValueKind.Object && x.TryGetProperty("score", out _))
                    .OrderByDescending(x => x.GetProperty("score").GetDouble())
                    .ToList();

                var best = ranked.FirstOrDefault();

                if (best.ValueKind != JsonValueKind.Object)
                    return ("Nötr", 0, false);

                var rawLabel = best.TryGetProperty("label", out var labelProp)
                    ? (labelProp.GetString() ?? string.Empty)
                    : string.Empty;

                var score = best.TryGetProperty("score", out var scoreProp)
                    ? scoreProp.GetDouble()
                    : 0;

                var label = NormalizeApiLabel(rawLabel);
                if (ShouldTreatAsNeutral(text, label, score, ranked))
                {
                    return ("Nötr", Math.Round(score, 4), true);
                }

                return (label, Math.Round(score, 4), true);
            }
            catch
            {
                return ("Nötr", 0, false);
            }
        }

        private static bool ShouldTreatAsNeutral(string text, string label, double score, List<JsonElement> ranked)
        {
            if (string.Equals(label, "Nötr", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (HasNeutralLanguagePattern(text))
            {
                return true;
            }

            if (score < NeutralConfidenceThreshold)
            {
                return true;
            }

            if (ranked.Count < 2)
            {
                return false;
            }

            var secondScore = ranked[1].TryGetProperty("score", out var secondScoreProp)
                ? secondScoreProp.GetDouble()
                : 0;

            return (score - secondScore) < NeutralMarginThreshold;
        }

        private static bool HasNeutralLanguagePattern(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var normalized = text.Trim().ToLowerInvariant();
            var neutralHits = NeutralCuePhrases.Count(normalized.Contains);
            var strongPositiveHits = StrongPositivePhrases.Count(normalized.Contains);
            var strongNegativeHits = StrongNegativePhrases.Count(normalized.Contains);

            if (normalized.Contains("ne çok") && normalized.Contains("ne de"))
            {
                return true;
            }

            if (neutralHits >= 2 && strongPositiveHits == 0 && strongNegativeHits == 0)
            {
                return true;
            }

            if (neutralHits >= 1 && strongPositiveHits > 0 && strongNegativeHits > 0)
            {
                return true;
            }

            return false;
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
