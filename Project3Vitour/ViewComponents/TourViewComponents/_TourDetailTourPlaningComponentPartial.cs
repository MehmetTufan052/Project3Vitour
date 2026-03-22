using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Helpers;
using Project3Vitour.Services.TourPlanService;
using Project3Vitour.Services.TourServices.ITourService;
using Project3Vitour.Services.TranslationService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailTourPlaningComponentPartial : ViewComponent
    {
        private readonly ITourPlanService _tourPlanService;
        private readonly ITourService _tourService;
        private readonly ITranslationService _translationService;

        public _TourDetailTourPlaningComponentPartial(ITourPlanService tourPlanService, ITourService tourService, ITranslationService translationService)
        {
            _tourPlanService = tourPlanService;
            _tourService = tourService;
            _translationService = translationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var values = await _tourPlanService.GetAllTourPlanAsync();
            var filtered = values.Where(x => x.TourId == tourId).ToList();

            if (!filtered.Any())
            {
                var tour = await _tourService.GetTourByIdAsync(tourId);
                if (!string.IsNullOrWhiteSpace(tour?.Title))
                {
                    filtered = values
                        .Where(x => string.Equals(x.TourName, tour.Title, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            filtered = await _translationService.LocalizeTourPlansAsync(filtered, RequestLanguageHelper.GetCurrentLanguage(HttpContext));
            return View(filtered);
        }
    }
}
