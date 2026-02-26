using AutoMapper;
using MongoDB.Driver;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Entities;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Review> _reviewCollection;
        public ReviewService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _reviewCollection = database.GetCollection<Review>(_databaseSettings.ReviewCollectionName);
            _mapper = mapper;
        }

        public async Task CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            var value = _mapper.Map<Review>(createReviewDto);
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
    }
}
