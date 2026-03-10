using Project3Vitour.Dtos.ReviewDto;

namespace Project3Vitour.Services.ReviewService
{
    public interface IReviewService
    {
        Task<List<ResultReviewDto>> GetAllReviewAsync();
        Task CreateReviewAsync(CreateReviewDto createReviewDto);
        Task UpdateReviewAsync(UpdateReviewDto updateReviewDto);
        Task DeleteReviewAsync(string id);
        Task<GetReviewByIdDto> GetReviewByIdAsync(string id);
        Task<List<ResultReviewByTourIdDto>> GetAllReviewsByTourIdAsync(string id);
        Task<List<ResultReviewDto>> AnalyzeAllReviewSentimentsAsync();
        Task<ResultReviewDto?> AnalyzeReviewSentimentAsync(string id);

        Task<SentimentSummaryDto> GetSentimentSummaryAsync(string tourId);
        Task<List<SentimentTrendDto>> GetMonthlyTrendAsync(string tourId);
    }
}