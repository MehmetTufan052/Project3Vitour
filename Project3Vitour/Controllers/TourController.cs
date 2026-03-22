using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Helpers;
using Project3Vitour.Services.TranslationService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourService _tourService;
        private readonly ITranslationService _translationService;

        public TourController(ITourService tourService, ITranslationService translationService)
        {
            _tourService = tourService;
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

            var value = await _tourService.GetTourByIdAsync(id);
            value = await _translationService.LocalizeTourAsync(value, languageCode);
            return View(value);
        }
    }
}
