using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.ReservationDtos;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Services.MailService;
using Project3Vitour.Services.ReservationService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ITourService _tourService;
        private readonly IEmailService _emailService;

        public ReservationController(IReservationService reservationService, ITourService tourService, IEmailService emailService)
        {
            _reservationService = reservationService;
            _tourService = tourService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateReservation()
        {
            var model = await BuildReservationToursAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto createReservationDto)
        {
            try
            {
                if (createReservationDto == null)
                {
                    return BadRequest(new { message = "Rezervasyon verisi bulunamadi." });
                }

                var tour = await _tourService.GetTourByIdAsync(createReservationDto.TourId);
                if (tour == null || !tour.IsStatus)
                {
                    return BadRequest(new { message = "Secilen tur bulunamadi." });
                }

                var reservations = await _reservationService.GetAllReservationAsync();

                var bookedCountsByTour = reservations
                    .GroupBy(x => x.TourId)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Sum(x => x.PersonCount));

                var alreadyBookedCount = bookedCountsByTour.TryGetValue(createReservationDto.TourId, out var bookedCount)
                    ? bookedCount
                    : 0;

                var availableCapacity = tour.Capacity - alreadyBookedCount;
                if (createReservationDto.PersonCount < 1)
                {
                    return BadRequest(new { message = "Kisi sayisi en az 1 olmali." });
                }

                if (availableCapacity < createReservationDto.PersonCount)
                {
                    return BadRequest(new
                    {
                        message = $"Secilen tur icin yeterli kontenjan yok. Kalan kapasite: {Math.Max(availableCapacity, 0)}"
                    });
                }

                createReservationDto.TotalPrice = tour.Price * createReservationDto.PersonCount;
                createReservationDto.ReservationId = null!;
                createReservationDto.ReservationCode = string.IsNullOrWhiteSpace(createReservationDto.ReservationCode)
                    ? GenerateReservationCode()
                    : createReservationDto.ReservationCode;

                await _reservationService.CreateReservationAsync(createReservationDto);

                var emailSent = true;
                string? emailError = null;

                try
                {
                    await _emailService.SendReservationCreatedEmailAsync(createReservationDto);
                }
                catch (Exception ex)
                {
                    emailSent = false;
                    emailError = ex.Message;
                }

                return Ok(new
                {
                    reservationCode = createReservationDto.ReservationCode,
                    totalPrice = createReservationDto.TotalPrice,
                    emailSent,
                    emailError
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private async Task<List<ResultTourDto>> BuildReservationToursAsync()
        {
            var tours = await _tourService.GetAllTourAsync();
            var reservations = await _reservationService.GetAllReservationAsync();

            var bookedCountsByTour = reservations
                .GroupBy(x => x.TourId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(x => x.PersonCount));

            var activeTours = tours
                .Where(x => x.IsStatus)
                .ToList();

            foreach (var tour in activeTours)
            {
                tour.BookedCount = bookedCountsByTour.TryGetValue(tour.TourId, out var bookedCount)
                    ? bookedCount
                    : 0;
            }

            return activeTours;
        }

        private static string GenerateReservationCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();

            string Segment(int length) =>
                new(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());

            return $"RVZ-{Segment(4)}-{Segment(4)}";
        }
    }
}
