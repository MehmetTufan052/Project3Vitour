using AutoMapper;
using MongoDB.Driver;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Entities;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Review> _reviewCollection;
        private readonly HuggingFaceService.HuggingFaceService _huggingFaceService;

        public ReviewService(IMapper mapper, IDatabaseSettings databaseSettings, HuggingFaceService.HuggingFaceService huggingFaceService)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _reviewCollection = database.GetCollection<Review>(databaseSettings.ReviewCollectionName);
            _mapper = mapper;
            _huggingFaceService = huggingFaceService;
        }

        public async Task CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            var value = _mapper.Map<Review>(createReviewDto);

            if (!string.IsNullOrWhiteSpace(value.Detail))
            {
                try
                {
                    var (label, score) = await _huggingFaceService.AnalyzeSentimentAsync(value.Detail);
                    value.SentimentLabel = NormalizeSentimentLabel(label);
                    value.SentimentScore = score;
                }
                catch
                {
                    value.SentimentLabel = "Nötr";
                    value.SentimentScore = 0;
                }
            }

            await _reviewCollection.InsertOneAsync(value);
        }

        public async Task DeleteReviewAsync(string id)
        {
            await _reviewCollection.DeleteOneAsync(x => x.ReviewId == id);
        }

        public async Task<List<ResultReviewDto>> GetAllReviewAsync()
        {
            var values = await _reviewCollection
                .Find(_ => true)
                .SortByDescending(x => x.ReviewDate)
                .ToListAsync();

            return values.Select(MapToResultReviewDto).ToList();
        }

        public async Task<List<ResultReviewDto>> AnalyzeAllReviewSentimentsAsync()
        {
            var reviews = await _reviewCollection
                .Find(x => !string.IsNullOrWhiteSpace(x.Detail))
                .ToListAsync();

            foreach (var review in reviews)
            {
                try
                {
                    var (label, score) = await _huggingFaceService.AnalyzeSentimentAsync(review.Detail);
                    review.SentimentLabel = NormalizeSentimentLabel(label);
                    review.SentimentScore = score;
                }
                catch
                {
                    review.SentimentLabel = "Nötr";
                    review.SentimentScore = 0;
                }

                await _reviewCollection.ReplaceOneAsync(x => x.ReviewId == review.ReviewId, review);
            }

            return (await _reviewCollection
                .Find(_ => true)
                .SortByDescending(x => x.ReviewDate)
                .ToListAsync())
                .Select(MapToResultReviewDto)
                .ToList();
        }

        public async Task<ResultReviewDto?> AnalyzeReviewSentimentAsync(string id)
        {
            var review = await _reviewCollection.Find(x => x.ReviewId == id).FirstOrDefaultAsync();
            if (review == null)
            {
                return null;
            }

            try
            {
                var (label, score) = await _huggingFaceService.AnalyzeSentimentAsync(review.Detail ?? string.Empty);
                review.SentimentLabel = NormalizeSentimentLabel(label);
                review.SentimentScore = score;
            }
            catch
            {
                review.SentimentLabel = "Nötr";
                review.SentimentScore = 0;
            }

            await _reviewCollection.ReplaceOneAsync(x => x.ReviewId == review.ReviewId, review);
            return MapToResultReviewDto(review);
        }

        public async Task<List<ResultReviewByTourIdDto>> GetAllReviewsByTourIdAsync(string id)
        {
            var values = await _reviewCollection.Find(x => x.TourId == id).ToListAsync();
            return _mapper.Map<List<ResultReviewByTourIdDto>>(values);
        }

        public async Task<GetReviewByIdDto> GetReviewByIdAsync(string id)
        {
            var value = await _reviewCollection.Find(x => x.ReviewId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetReviewByIdDto>(value);
        }

        public async Task UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            var value = _mapper.Map<Review>(updateReviewDto);
            await _reviewCollection.FindOneAndReplaceAsync(x => x.ReviewId == updateReviewDto.ReviewId, value);
        }

        public async Task<SentimentSummaryDto> GetSentimentSummaryAsync(string tourId)
        {
            var reviews = await _reviewCollection
                .Find(r => r.TourId == tourId && r.SentimentLabel != null)
                .ToListAsync();

            return new SentimentSummaryDto
            {
                TourId = tourId,
                ToplamYorum = reviews.Count,
                OlumluSayi = reviews.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Olumlu"),
                OlumsuzSayi = reviews.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Olumsuz"),
                NotSayi = reviews.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Nötr"),
                OrtalamaGuven = reviews.Any() ? Math.Round(reviews.Average(r => r.SentimentScore), 2) : 0
            };
        }

        public async Task<List<SentimentTrendDto>> GetMonthlyTrendAsync(string tourId)
        {
            var reviews = await _reviewCollection
                .Find(r => r.TourId == tourId && r.SentimentLabel != null)
                .ToListAsync();

            return reviews
                .GroupBy(r => new { r.ReviewDate.Year, r.ReviewDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new SentimentTrendDto
                {
                    Ay = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Olumlu = g.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Olumlu"),
                    Olumsuz = g.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Olumsuz"),
                    Notr = g.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Nötr")
                })
                .ToList();
        }

        private static string NormalizeSentimentLabel(string? label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return "Nötr";
            }

            var normalized = label.Trim().ToLowerInvariant();
            return normalized switch
            {
                "olumlu" or "positive" => "Olumlu",
                "olumsuz" or "negative" => "Olumsuz",
                "nötr" or "notr" or "neutral" => "Nötr",
                _ => "Nötr"
            };
        }

        private static ResultReviewDto MapToResultReviewDto(Review x)
        {
            return new ResultReviewDto
            {
                ReviewId = x.ReviewId,
                NameSurname = x.NameSurname,
                Detail = x.Detail,
                Score = x.Score,
                ReviewDate = x.ReviewDate,
                Status = x.Status,
                TourId = x.TourId,
                SentimentLabel = NormalizeSentimentLabel(x.SentimentLabel),
                SentimentScore = x.SentimentScore
            };
        }
    }
}