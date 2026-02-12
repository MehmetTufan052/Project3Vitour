using AutoMapper;
using Project3Vitour.Dtos.TourDtos;

namespace Project3Vitour.Services.TourServices.ITourService
{
    public class TourService : ITourService
    {
        private readonly IMapper _mapper;

        public TourService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task CreateTourAsync(CreateTourDto createTourDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTourAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResultTourDto>> GetAllTourAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GetTourByIdDto> GetTourByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTourAsync(UpdateTourDto updateTourDto)
        {
            throw new NotImplementedException();
        }
    }
}
