using Project3Vitour.Dtos.ReservationDtos;

namespace Project3Vitour.Services.MailService
{
    public interface IEmailService
    {
        Task SendReservationCreatedEmailAsync(CreateReservationDto reservation, CancellationToken cancellationToken = default);
    }
}
