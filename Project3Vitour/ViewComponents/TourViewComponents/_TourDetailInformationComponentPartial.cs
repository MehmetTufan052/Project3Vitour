using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.ReviewService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailInformationComponentPartial : ViewComponent
    {
        private readonly IReviewService _reviewService;

        public _TourDetailInformationComponentPartial(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var reviews = await _reviewService.GetAllReviewAsync();
            ViewBag.ReviewCount = reviews.Count(x => x.TourId == tourId);
            return View();
        }
    }
}
