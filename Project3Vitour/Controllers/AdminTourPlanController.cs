using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.TourPlanDto;
using Project3Vitour.Services.TourPlanService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.Controllers
{
    public class AdminTourPlanController : Controller
    {
        private readonly ITourPlanService _tourPlanService;
        private readonly ITourService _tourService;

        public AdminTourPlanController(ITourPlanService tourPlanService, ITourService tourService)
        {
            _tourPlanService = tourPlanService;
            _tourService = tourService;
        }


        public async Task<IActionResult> TourPlanList()
        {
            var values = await _tourPlanService.GetAllTourPlanAsync();
            return View(values);
        }

        public async Task<IActionResult> DeleteTourPlan(string id)
        {
            await _tourPlanService.DeleteTourPlanAsync(id);
            return RedirectToAction("TourPlanList");
        }


        [HttpGet]
        public async Task<IActionResult> CreateTourPlan()
        {
            var tours = await _tourService.GetAllTourAsync();
            ViewBag.Tours = tours;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTourPlan(CreateTourPlanDto dto)
        {
            await _tourPlanService.CreateTourPlanAsync(dto);
            return RedirectToAction("TourPlanList");
        }
    }
}
