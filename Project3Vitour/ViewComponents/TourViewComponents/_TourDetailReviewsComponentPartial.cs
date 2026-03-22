using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Helpers;
using Project3Vitour.Services.ReviewService;
using Project3Vitour.Services.TranslationService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailReviewsComponentPartial : ViewComponent
    {
        private readonly IReviewService _reviewService;
        private readonly ITranslationService _translationService;

        public _TourDetailReviewsComponentPartial(IReviewService reviewService, ITranslationService translationService)
        {
            _reviewService = reviewService;
            _translationService = translationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var values = await _reviewService.GetAllReviewAsync();
            values = await _translationService.LocalizeReviewsAsync(values, RequestLanguageHelper.GetCurrentLanguage(HttpContext));
            var filtered = values.Where(x => x.TourId == tourId).ToList();
            ViewBag.TourId = tourId;
            return View(filtered);
        }
    }
}
