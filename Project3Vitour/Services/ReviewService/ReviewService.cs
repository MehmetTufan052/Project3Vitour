using AutoMapper;
using Project3Vitour.Dtos.ReviewDto;

namespace Project3Vitour.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        public Task CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteReviewAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResultReviewDto>> GetAllReviewAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GetReviewByIdDto> GetReviewByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            throw new NotImplementedException();
        }
    }
}
