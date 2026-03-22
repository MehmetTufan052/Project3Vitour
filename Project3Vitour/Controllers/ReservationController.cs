using Humanizer;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Project3Vitour.Dtos.ReservationDtos;
using Project3Vitour.Entities;
using Project3Vitour.Services.ReservationService;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Project3Vitour.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
           _reservationService = reservationService;
        }

        [HttpGet]
        public IActionResult CreateReservation()
        {
            return View(); 
        }
       
        [HttpPost]
        public IActionResult CreateReservation(CreateReservationDto createReservationDto)
        {
            decimal tourPrice = tour.Price;
            var code = await _reservationService.CreateReservationAsync(dto, tourPrice);

            MimeMessage mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress("Vitour", "tufaneser8@gmail.com"));
            mimeMessage.To.Add(new MailboxAddress("User", createReservationDto.Email));
            mimeMessage.Subject =$"Rezervasyonunuz Onaylandı – Kod: {code}";
            return View();
        }
    }
}
