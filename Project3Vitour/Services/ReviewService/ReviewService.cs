using AutoMapper;
using MongoDB.Driver;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Entities;
using Project3Vitour.Services.HuggingFaceService;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Review> _reviewCollection;
        private readonly HuggingFaceService.HuggingFaceService _huggingFaceService;
        public ReviewService(IMapper mapper, IDatabaseSettings _databaseSettings, HuggingFaceService.HuggingFaceService huggingFaceService)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _reviewCollection = database.GetCollection<Review>(_databaseSettings.ReviewCollectionName);
            _mapper = mapper;
            _huggingFaceService = huggingFaceService;
        }

        public async Task CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            var value = _mapper.Map<Review>(createReviewDto);
           
            if (!string.IsNullOrWhiteSpace(value.Detail))
            {
                var (label, score) = await _huggingFaceService.AnalyzeSentimentAsync(value.Detail);
                value.SentimentLabel = label;
                value.SentimentScore = score;
            }

            await _reviewCollection.InsertOneAsync(value);
        }

        public async Task DeleteReviewAsync(string id)
        {
            await _reviewCollection.DeleteOneAsync(x => x.ReviewId == id);
        }

        public async Task<List<ResultReviewDto>> GetAllReviewAsync()
        {
            var values = await _reviewCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultReviewDto>>(values);
        }

        public async Task<List<ResultReviewByTourIdDto>> GetAllReviewsByTourIdAsync(string id)
        {
            var values=await _reviewCollection.Find(x=>x.TourId==id).ToListAsync();
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

        // Admin panel için sentiment özet
        public async Task<SentimentSummaryDto> GetSentimentSummaryAsync(string tourId)
        {
            var reviews = await _reviewCollection
                .Find(r => r.TourId == tourId && r.SentimentLabel != null)
                .ToListAsync();

            return new SentimentSummaryDto
            {
                TourId = tourId,
                ToplamYorum = reviews.Count,
                OlumluSayi = reviews.Count(r => r.SentimentLabel == "Olumlu"),
                OlumsuzSayi = reviews.Count(r => r.SentimentLabel == "Olumsuz"),
                NotSayi = reviews.Count(r => r.SentimentLabel == "Nötr"),
                OrtalamaGuven = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.SentimentScore), 2)
                    : 0
            };
        }

        // Admin panel için aylık trend
        public async Task<List<SentimentTrendDto>> GetMonthlyTrendAsync(string tourId)
        {
            var reviews = await _reviewCollection
                .Find(r => r.TourId == tourId && r.SentimentLabel != null)
                .ToListAsync();

            return reviews
                .GroupBy(r => new { r.ReviewDate.Year, r.ReviewDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new SentimentTrendDto
                {
                    Ay = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Olumlu = g.Count(r => r.SentimentLabel == "Olumlu"),
                    Olumsuz = g.Count(r => r.SentimentLabel == "Olumsuz"),
                    Notr = g.Count(r => r.SentimentLabel == "Nötr")
                })
                .ToList();
        }
    }
}

