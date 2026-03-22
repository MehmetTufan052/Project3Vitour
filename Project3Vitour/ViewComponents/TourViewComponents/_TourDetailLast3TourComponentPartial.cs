using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Helpers;
using Project3Vitour.Services.TourServices.ITourService;
using Project3Vitour.Services.TranslationService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailLast3TourComponentPartial : ViewComponent
    {
        private readonly ITourService _tourService;
        private readonly ITranslationService _translationService;

        public _TourDetailLast3TourComponentPartial(ITourService tourService, ITranslationService translationService)
        {
            _tourService = tourService;
            _translationService = translationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _tourService.GetAllTourAsync();
            var last3 = values.TakeLast(3).ToList();
            last3 = await _translationService.LocalizeToursAsync(last3, RequestLanguageHelper.GetCurrentLanguage(HttpContext));
            return View(last3);
        }
    }
}
