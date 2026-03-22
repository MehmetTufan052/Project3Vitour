using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Dtos.TourPlanDto;

namespace Project3Vitour.Services.TranslationService
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string entityType, string entityId, string fieldName, string sourceText, string targetLanguageCode, bool isHtml = false, CancellationToken cancellationToken = default);
        Task<GetTourByIdDto?> LocalizeTourAsync(GetTourByIdDto? tour, string languageCode, CancellationToken cancellationToken = default);
        Task<List<ResultTourDto>> LocalizeToursAsync(IEnumerable<ResultTourDto> tours, string languageCode, CancellationToken cancellationToken = default);
        Task<List<ResultTourPlanDto>> LocalizeTourPlansAsync(IEnumerable<ResultTourPlanDto> plans, string languageCode, CancellationToken cancellationToken = default);
        Task<List<ResultReviewDto>> LocalizeReviewsAsync(IEnumerable<ResultReviewDto> reviews, string languageCode, CancellationToken cancellationToken = default);
        Task<List<ResultReviewByTourIdDto>> LocalizeReviewsByTourAsync(IEnumerable<ResultReviewByTourIdDto> reviews, string languageCode, CancellationToken cancellationToken = default);
    }
}
