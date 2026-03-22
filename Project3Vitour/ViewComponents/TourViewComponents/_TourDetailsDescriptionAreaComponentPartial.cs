using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Helpers;
using Project3Vitour.Services.TourServices.ITourService;
using Project3Vitour.Services.TranslationService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailsDescriptionAreaComponentPartial : ViewComponent
    {
        private readonly ITourService _tourService;
        private readonly ITranslationService _translationService;

        public _TourDetailsDescriptionAreaComponentPartial(ITourService tourService, ITranslationService translationService)
        {
            _tourService = tourService;
            _translationService = translationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var value = await _tourService.GetTourByIdAsync(tourId);
            value = await _translationService.LocalizeTourAsync(value, RequestLanguageHelper.GetCurrentLanguage(HttpContext));
            return View(value);
        }
    }
}
