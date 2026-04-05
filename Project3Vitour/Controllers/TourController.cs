using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Dtos.TourPlanDto;
using Project3Vitour.Helpers;
using Project3Vitour.Models;
using Project3Vitour.Services.TourPlanService;
using Project3Vitour.Services.TranslationService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourService _tourService;
        private readonly ITourPlanService _tourPlanService;
        private readonly ITranslationService _translationService;

        public TourController(ITourService tourService, ITourPlanService tourPlanService, ITranslationService translationService)
        {
            _tourService = tourService;
            _tourPlanService = tourPlanService;
            _translationService = translationService;
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
            var languageCode = RequestLanguageHelper.GetCurrentLanguage(HttpContext);
            TranslationDebugLogger.Log($"TourController.TourDetails id={id} lang={languageCode} queryLang={HttpContext.Request.Query["lang"]} cookieLang={HttpContext.Request.Cookies["site_lang"]}");

            var tour = await _tourService.GetTourByIdAsync(id);
            tour = await _translationService.LocalizeTourAsync(tour, languageCode);

            var plans = await GetLocalizedTourPlansAsync(id, tour, languageCode);

            return View(new TourDetailsViewModel
            {
                Tour = tour,
                TourPlans = plans
            });
        }

        private async Task<List<ResultTourPlanDto>> GetLocalizedTourPlansAsync(string tourId, GetTourByIdDto? localizedTour, string languageCode)
        {
            var plans = await _tourPlanService.GetAllTourPlanAsync();
            var filtered = plans.Where(x => x.TourId == tourId).ToList();

            if (!filtered.Any() && !string.IsNullOrWhiteSpace(localizedTour?.Title))
            {
                filtered = plans
                    .Where(x => string.Equals(x.TourName, localizedTour.Title, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!filtered.Any())
            {
                var sourceTour = await _tourService.GetTourByIdAsync(tourId);
                if (!string.IsNullOrWhiteSpace(sourceTour?.Title))
                {
                    filtered = plans
                        .Where(x => string.Equals(x.TourName, sourceTour.Title, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            return await _translationService.LocalizeTourPlansAsync(filtered, languageCode);
        }
    }
}
