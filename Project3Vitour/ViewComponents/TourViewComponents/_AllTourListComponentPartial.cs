using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Helpers;
using Project3Vitour.Services.TourServices.ITourService;
using Project3Vitour.Services.TranslationService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _AllTourListComponentPartial : ViewComponent
    {
        private readonly ITourService _tourService;
        private readonly ITranslationService _translationService;

        public _AllTourListComponentPartial(ITourService tourService, ITranslationService translationService)
        {
            _tourService = tourService;
            _translationService = translationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tours = await _tourService.GetAllTourAsync();
            tours = await _translationService.LocalizeToursAsync(tours, RequestLanguageHelper.GetCurrentLanguage(HttpContext));
            return View(tours);
        }
    }
}
