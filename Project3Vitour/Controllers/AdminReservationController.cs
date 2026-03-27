using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.ReservationService;

namespace Project3Vitour.Controllers
{
    public class AdminReservationController : Controller
    {
        private readonly IReservationService _reservationService;

        public AdminReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task<IActionResult> ReservationList()
        {
            var reservationList= await _reservationService.GetAllReservationAsync();
            return View(reservationList);
        }
    }
}
