using Project3Vitour.Dtos.TourDtos;

namespace Project3Vitour.Services.TourServices.ITourService
{
    public interface ITourService
    {
        Task<List<ResultTourDto>> GetAllTourAsync();
        Task CreateTourAsync(CreateTourDto createTourDto);
        Task UpdateTourAsync(UpdateTourDto updateTourDto);
        Task DeleteTourAsync(string id);

        Task<GetTourByIdDto> GetTourByIdAsync(string id);
    }
}
