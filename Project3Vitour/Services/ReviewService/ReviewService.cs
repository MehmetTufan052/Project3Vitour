using AutoMapper;
using MongoDB.Driver;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Entities;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private const string NeutralLabel = "Nötr";

        private readonly IMapper _mapper;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Review> _reviewCollection;
        private readonly HuggingFaceService.HuggingFaceService _huggingFaceService;

        public ReviewService(IMapper mapper, IDatabaseSettings databaseSettings, HuggingFaceService.HuggingFaceService huggingFaceService)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            _database = client.GetDatabase(databaseSettings.DatabaseName);
            _reviewCollection = _database.GetCollection<Review>(databaseSettings.ReviewCollectionName);
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
                    var (label, score, success) = await _huggingFaceService.AnalyzeSentimentAsync(value.Detail);
                    if (success)
                    {
                        value.SentimentLabel = NormalizeSentimentLabel(label);
                        value.SentimentScore = score;
                    }
                    else
                    {
                        value.SentimentLabel = NeutralLabel;
                        value.SentimentScore = 0;
                    }
                }
                catch
                {
                    value.SentimentLabel = NeutralLabel;
                    value.SentimentScore = 0;
                }
            }

            await _reviewCollection.InsertOneAsync(value);
        }

        public async Task DeleteReviewAsync(string id)
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var result = await activeCollection.DeleteOneAsync(x => x.ReviewId == id);

            if (result.DeletedCount == 0 && activeCollection.CollectionNamespace.CollectionName != _reviewCollection.CollectionNamespace.CollectionName)
            {
                await _reviewCollection.DeleteOneAsync(x => x.ReviewId == id);
            }
        }

        public async Task<List<ResultReviewDto>> GetAllReviewAsync()
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var values = await activeCollection
                .Find(_ => true)
                .SortByDescending(x => x.ReviewDate)
                .ToListAsync();

            return values.Select(MapToResultReviewDto).ToList();
        }

        public async Task<List<ResultReviewDto>> AnalyzeAllReviewSentimentsAsync()
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var reviews = await activeCollection
                .Find(x => !string.IsNullOrWhiteSpace(x.Detail))
                .ToListAsync();

            foreach (var review in reviews)
            {
                try
                {
                    var (label, score, success) = await _huggingFaceService.AnalyzeSentimentAsync(review.Detail ?? string.Empty);
                    if (success)
                    {
                        review.SentimentLabel = NormalizeSentimentLabel(label);
                        review.SentimentScore = score;
                    }
                    else
                    {
                        review.SentimentLabel = NeutralLabel;
                        review.SentimentScore = 0;
                    }
                }
                catch
                {
                    review.SentimentLabel = NeutralLabel;
                    review.SentimentScore = 0;
                }

                await activeCollection.ReplaceOneAsync(x => x.ReviewId == review.ReviewId, review);
            }

            return (await activeCollection
                    .Find(_ => true)
                    .SortByDescending(x => x.ReviewDate)
                    .ToListAsync())
                .Select(MapToResultReviewDto)
                .ToList();
        }

        public async Task<ResultReviewDto?> AnalyzeReviewSentimentAsync(string id)
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var review = await activeCollection.Find(x => x.ReviewId == id).FirstOrDefaultAsync();

            if (review == null)
            {
                review = await _reviewCollection.Find(x => x.ReviewId == id).FirstOrDefaultAsync();
                if (review == null)
                {
                    return null;
                }

                activeCollection = _reviewCollection;
            }

            try
            {
                var (label, score, success) = await _huggingFaceService.AnalyzeSentimentAsync(review.Detail ?? string.Empty);
                if (success)
                {
                    review.SentimentLabel = NormalizeSentimentLabel(label);
                    review.SentimentScore = score;
                }
                else
                {
                    review.SentimentLabel = NeutralLabel;
                    review.SentimentScore = 0;
                }
            }
            catch
            {
                review.SentimentLabel = NeutralLabel;
                review.SentimentScore = 0;
            }

            await activeCollection.ReplaceOneAsync(x => x.ReviewId == review.ReviewId, review);
            return MapToResultReviewDto(review);
        }

        public async Task<List<ResultReviewByTourIdDto>> GetAllReviewsByTourIdAsync(string id)
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var values = await activeCollection.Find(x => x.TourId == id).ToListAsync();
            return _mapper.Map<List<ResultReviewByTourIdDto>>(values);
        }

        public async Task<GetReviewByIdDto> GetReviewByIdAsync(string id)
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var value = await activeCollection.Find(x => x.ReviewId == id).FirstOrDefaultAsync();

            if (value == null && activeCollection.CollectionNamespace.CollectionName != _reviewCollection.CollectionNamespace.CollectionName)
            {
                value = await _reviewCollection.Find(x => x.ReviewId == id).FirstOrDefaultAsync();
            }

            return _mapper.Map<GetReviewByIdDto>(value);
        }

        public async Task UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var value = _mapper.Map<Review>(updateReviewDto);
            await activeCollection.FindOneAndReplaceAsync(x => x.ReviewId == updateReviewDto.ReviewId, value);
        }

        public async Task<SentimentSummaryDto> GetSentimentSummaryAsync(string tourId)
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var reviews = await activeCollection
                .Find(r => r.TourId == tourId && r.SentimentLabel != null)
                .ToListAsync();

            return new SentimentSummaryDto
            {
                TourId = tourId,
                ToplamYorum = reviews.Count,
                OlumluSayi = reviews.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Olumlu"),
                OlumsuzSayi = reviews.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == "Olumsuz"),
                NotSayi = reviews.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == NeutralLabel),
                OrtalamaGuven = reviews.Any() ? Math.Round(reviews.Average(r => r.SentimentScore), 2) : 0
            };
        }

        public async Task<List<SentimentTrendDto>> GetMonthlyTrendAsync(string tourId)
        {
            var activeCollection = await ResolveActiveCollectionAsync();
            var reviews = await activeCollection
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
                    Notr = g.Count(r => NormalizeSentimentLabel(r.SentimentLabel) == NeutralLabel)
                })
                .ToList();
        }

        private async Task<IMongoCollection<Review>> ResolveActiveCollectionAsync()
        {
            var primaryCount = await _reviewCollection.CountDocumentsAsync(_ => true);
            if (primaryCount > 0)
            {
                return _reviewCollection;
            }

            var alternativeNames = new[] { "Review", "Reviews" }
                .Where(name => !string.Equals(name, _reviewCollection.CollectionNamespace.CollectionName, StringComparison.OrdinalIgnoreCase));

            foreach (var name in alternativeNames)
            {
                var altCollection = _database.GetCollection<Review>(name);
                var altCount = await altCollection.CountDocumentsAsync(_ => true);
                if (altCount > 0)
                {
                    return altCollection;
                }
            }

            var allCollectionNames = await _database.ListCollectionNames().ToListAsync();
            foreach (var collectionName in allCollectionNames.Where(x => x.Contains("review", StringComparison.OrdinalIgnoreCase)))
            {
                var candidate = _database.GetCollection<Review>(collectionName);
                var candidateCount = await candidate.CountDocumentsAsync(_ => true);
                if (candidateCount > 0)
                {
                    return candidate;
                }
            }

            return _reviewCollection;
        }

        private static string NormalizeSentimentLabel(string? label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return NeutralLabel;
            }

            var normalized = label.Trim().ToLowerInvariant();
            return normalized switch
            {
                "olumlu" or "positive" or "label_1" => "Olumlu",
                "olumsuz" or "negative" or "label_0" => "Olumsuz",
                "nötr" or "notr" or "neutral" or "label_2" => NeutralLabel,
                _ => NeutralLabel
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
