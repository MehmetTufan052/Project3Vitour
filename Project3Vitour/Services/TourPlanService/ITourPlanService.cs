using Project3Vitour.Dtos.TourPlanDto;

namespace Project3Vitour.Services.TourPlanService
{
    public interface ITourPlanService
    {
        Task CreateTourPlanAsync(CreateTourPlanDto createTourPlanDto);
        Task DeleteTourPlanAsync(string id);
        Task UpdateTourPlanAsync(UpdateTourPlanDto updateTourPlanDto);
        Task<List<ResultTourPlanDto>> GetAllTourPlanAsync();
        Task<GetTourPlanByIdDto> GetTourPlanByIdAsync(string id);

        
    }
}
