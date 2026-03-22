using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Helpers;
using Project3Vitour.Services.ReviewService;
using Project3Vitour.Services.TourServices.ITourService;
using Project3Vitour.Services.TranslationService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailInformationComponentPartial : ViewComponent
    {
        private readonly IReviewService _reviewService;
        private readonly ITourService _tourService;
        private readonly ITranslationService _translationService;

        public _TourDetailInformationComponentPartial(IReviewService reviewService, ITourService tourService, ITranslationService translationService)
        {
            _reviewService = reviewService;
            _tourService = tourService;
            _translationService = translationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var languageCode = RequestLanguageHelper.GetCurrentLanguage(HttpContext);
            TranslationDebugLogger.Log($"_TourDetailInformationComponentPartial.InvokeAsync tourId={tourId} lang={languageCode} queryLang={HttpContext?.Request.Query["lang"]} cookieLang={HttpContext?.Request.Cookies["site_lang"]}");

            var reviews = await _reviewService.GetAllReviewAsync();
            ViewBag.ReviewCount = reviews.Count(x => x.TourId == tourId);

            var tour = await _tourService.GetTourByIdAsync(tourId);
            tour = await _translationService.LocalizeTourAsync(tour, languageCode);

            return View(tour);
        }
    }
}
