using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Dtos.TourPlanDto;
using Project3Vitour.Entities;
using Project3Vitour.Helpers;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.TranslationService
{
    public class TranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly IMongoCollection<Translation> _translationCollection;
        private readonly TranslationSettings _settings;

        public TranslationService(HttpClient httpClient, IDatabaseSettings databaseSettings, Microsoft.Extensions.Options.IOptions<TranslationSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _translationCollection = database.GetCollection<Translation>(databaseSettings.TranslationCollectionName);
        }

        public async Task<string> TranslateAsync(string entityType, string entityId, string fieldName, string sourceText, string targetLanguageCode, bool isHtml = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sourceText) || string.Equals(targetLanguageCode, "tr", StringComparison.OrdinalIgnoreCase))
            {
                TranslationDebugLogger.Log($"TranslateAsync skip entity={entityType} entityId={entityId} field={fieldName} lang={targetLanguageCode} reason={(string.IsNullOrWhiteSpace(sourceText) ? "empty-source" : "target-tr")}");
                return sourceText;
            }

            var normalizedTarget = targetLanguageCode.Trim().ToLowerInvariant();
            TranslationDebugLogger.Log($"TranslateAsync start entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} baseUrl={_settings.BaseUrl} sourcePreview={Preview(sourceText)}");

            var existing = await _translationCollection.Find(x =>
                    x.EntityType == entityType &&
                    x.EntityId == entityId &&
                    x.FieldName == fieldName &&
                    x.LanguageCode == normalizedTarget)
                .FirstOrDefaultAsync(cancellationToken);

            if (existing != null && !string.IsNullOrWhiteSpace(existing.Value))
            {
                TranslationDebugLogger.Log($"TranslateAsync cache-hit entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} valuePreview={Preview(existing.Value)}");
                return existing.Value;
            }

            if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
            {
                TranslationDebugLogger.Log($"TranslateAsync fallback entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} reason=empty-base-url");
                return sourceText;
            }

            var endpoint = BuildTranslateEndpoint();
            var payload = new Dictionary<string, object?>
            {
                ["q"] = sourceText,
                ["source"] = _settings.DefaultSourceLanguage,
                ["target"] = normalizedTarget,
                ["format"] = isHtml ? "html" : "text",
                ["api_key"] = string.IsNullOrWhiteSpace(_settings.ApiKey) ? null : _settings.ApiKey
            };
            var payloadJson = JsonSerializer.Serialize(payload);
            TranslationDebugLogger.Log($"TranslateAsync payload entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} payloadPreview={Preview(payloadJson)}");

            HttpResponseMessage response;
            try
            {
                using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
                TranslationDebugLogger.Log($"TranslateAsync http entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} endpoint={endpoint} status={(int)response.StatusCode}");
            }
            catch (Exception ex)
            {
                TranslationDebugLogger.Log($"TranslateAsync exception entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} endpoint={endpoint} error={ex.GetType().Name}:{ex.Message}");
                return sourceText;
            }

            if (!response.IsSuccessStatusCode)
            {
                TranslationDebugLogger.Log($"TranslateAsync fallback entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} reason=http-failure");
                return sourceText;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            TranslationDebugLogger.Log($"TranslateAsync body entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} bodyPreview={Preview(responseContent)}");

            var translatedText = ExtractTranslatedText(responseContent);

            if (string.IsNullOrWhiteSpace(translatedText))
            {
                TranslationDebugLogger.Log($"TranslateAsync fallback entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} reason=empty-response");
                return sourceText;
            }

            var translation = existing ?? new Translation
            {
                EntityType = entityType,
                EntityId = entityId,
                FieldName = fieldName,
                LanguageCode = normalizedTarget,
                SourceLanguageCode = _settings.DefaultSourceLanguage
            };

            translation.Value = translatedText;
            translation.IsHtml = isHtml;
            translation.UpdatedAtUtc = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(translation.TranslationId))
            {
                await _translationCollection.InsertOneAsync(translation, cancellationToken: cancellationToken);
            }
            else
            {
                await _translationCollection.ReplaceOneAsync(x => x.TranslationId == translation.TranslationId, translation, cancellationToken: cancellationToken);
            }

            TranslationDebugLogger.Log($"TranslateAsync success entity={entityType} entityId={entityId} field={fieldName} lang={normalizedTarget} valuePreview={Preview(translatedText)}");
            return translatedText;
        }

        public async Task<GetTourByIdDto?> LocalizeTourAsync(GetTourByIdDto? tour, string languageCode, CancellationToken cancellationToken = default)
        {
            if (tour == null || languageCode == "tr")
            {
                return tour;
            }

            tour.Title = await TranslateAsync("Tour", tour.TourId, "Title", tour.Title, languageCode, cancellationToken: cancellationToken);
            tour.Description = await TranslateAsync("Tour", tour.TourId, "Description", tour.Description, languageCode, true, cancellationToken);
            tour.LongDescription = await TranslateAsync("Tour", tour.TourId, "LongDescription", tour.LongDescription, languageCode, true, cancellationToken);
            tour.Badge = await TranslateAsync("Tour", tour.TourId, "Badge", tour.Badge, languageCode, cancellationToken: cancellationToken);
            tour.LocationPageTitle = await TranslateAsync("Tour", tour.TourId, "LocationPageTitle", tour.LocationPageTitle, languageCode, cancellationToken: cancellationToken);
            tour.LocationPageDescription = await TranslateAsync("Tour", tour.TourId, "LocationPageDescription", tour.LocationPageDescription, languageCode, cancellationToken: cancellationToken);

            return tour;
        }

        public async Task<List<ResultTourDto>> LocalizeToursAsync(IEnumerable<ResultTourDto> tours, string languageCode, CancellationToken cancellationToken = default)
        {
            var localized = new List<ResultTourDto>();
            foreach (var tour in tours)
            {
                if (languageCode != "tr")
                {
                    tour.Title = await TranslateAsync("Tour", tour.TourId, "Title", tour.Title, languageCode, cancellationToken: cancellationToken);
                    tour.Description = await TranslateAsync("Tour", tour.TourId, "Description", tour.Description, languageCode, cancellationToken: cancellationToken);
                    tour.Badge = await TranslateAsync("Tour", tour.TourId, "Badge", tour.Badge, languageCode, cancellationToken: cancellationToken);
                }

                localized.Add(tour);
            }

            return localized;
        }

        public async Task<List<ResultTourPlanDto>> LocalizeTourPlansAsync(IEnumerable<ResultTourPlanDto> plans, string languageCode, CancellationToken cancellationToken = default)
        {
            var localized = new List<ResultTourPlanDto>();
            foreach (var plan in plans)
            {
                if (languageCode != "tr")
                {
                    plan.DayTitle = await TranslateAsync("TourPlan", plan.TourPlanId, "DayTitle", plan.DayTitle, languageCode, cancellationToken: cancellationToken);
                    plan.Description = await TranslateAsync("TourPlan", plan.TourPlanId, "Description", plan.Description, languageCode, true, cancellationToken);
                }

                localized.Add(plan);
            }

            return localized;
        }

        public async Task<List<ResultReviewDto>> LocalizeReviewsAsync(IEnumerable<ResultReviewDto> reviews, string languageCode, CancellationToken cancellationToken = default)
        {
            var localized = new List<ResultReviewDto>();
            foreach (var review in reviews)
            {
                if (languageCode != "tr")
                {
                    review.Detail = await TranslateAsync("Review", review.ReviewId, "Detail", review.Detail, languageCode, cancellationToken: cancellationToken);
                }

                localized.Add(review);
            }

            return localized;
        }

        public async Task<List<ResultReviewByTourIdDto>> LocalizeReviewsByTourAsync(IEnumerable<ResultReviewByTourIdDto> reviews, string languageCode, CancellationToken cancellationToken = default)
        {
            var localized = new List<ResultReviewByTourIdDto>();
            foreach (var review in reviews)
            {
                if (languageCode != "tr")
                {
                    review.Detail = await TranslateAsync("Review", review.ReviewId, "Detail", review.Detail, languageCode, cancellationToken: cancellationToken);
                }

                localized.Add(review);
            }

            return localized;
        }

        private string BuildTranslateEndpoint()
        {
            var baseUrl = string.IsNullOrWhiteSpace(_settings.BaseUrl)
                ? string.Empty
                : _settings.BaseUrl.TrimEnd('/');

            return $"{baseUrl}/translate";
        }

        private static string Preview(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "<empty>";
            }

            var normalized = value.Replace(Environment.NewLine, " ").Replace('\n', ' ').Replace('\r', ' ').Trim();
            return normalized.Length <= 80 ? normalized : normalized[..80];
        }

        private static string? ExtractTranslatedText(string? responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return null;
            }

            try
            {
                using var document = JsonDocument.Parse(responseContent);
                if (document.RootElement.ValueKind == JsonValueKind.Object &&
                    document.RootElement.TryGetProperty("translatedText", out var translatedTextElement))
                {
                    return translatedTextElement.GetString();
                }
            }
            catch
            {
            }

            return null;
        }

    }
}
