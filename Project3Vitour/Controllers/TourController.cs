using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourService _tourService;



        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        public IActionResult CreateTour()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTour(CreateTourDto createTourDto)
        {
            await _tourService.CreateTourAsync(createTourDto);
            return RedirectToAction("TourList");
        }
        public IActionResult TourList()
        {
            return View();
        }

        public async Task<IActionResult> TourDetails(string id)
        {
            var value=await _tourService.GetTourByIdAsync(id);
            return View(value);
        }
    }
}
