using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.ReviewService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailReviewsComponentPartial : ViewComponent
    {
        private readonly IReviewService _reviewService;

        public _TourDetailReviewsComponentPartial(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var values = await _reviewService.GetAllReviewAsync();
            var filtered = values.Where(x => x.TourId == tourId).ToList();
            ViewBag.TourId = tourId;
            return View(filtered);
        }
    }
}
