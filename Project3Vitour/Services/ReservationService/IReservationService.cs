using Project3Vitour.Dtos.ReservationDtos;

namespace Project3Vitour.Services.ReservationService
{
    public interface IReservationService
    {
        Task<List<ResultReservationDto>> GetAllReservationAsync();
        Task CreateReservationAsync(CreateReservationDto createReservationDto);
        Task UpdateReservationAsync(UpdateReservationDto updateReservationDto);
        Task DeleteReservationAsync(string id);
        Task <GetReservationByIdDto> GetReservationByIdAsync(string id);
    }
}
