using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.GalleryService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.Controllers
{
    public class AdminGalleryController : Controller
    {
        private readonly IGalleryService _galleryService;
        private readonly ITourService _tourService;
        public AdminGalleryController(IGalleryService galleryService, ITourService tourService)
        {
            _galleryService = galleryService;
            _tourService = tourService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateGallery()
        {
            var tours = await _tourService.GetAllTourAsync();
            ViewBag.Tours = tours;
            return View();
        }
    }
}
