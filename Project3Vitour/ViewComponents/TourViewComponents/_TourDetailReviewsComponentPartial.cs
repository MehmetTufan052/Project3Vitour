using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Entities;
using Project3Vitour.Services.ReviewService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailReviewsComponentPartial : ViewComponent
    {
        IReviewService _reviewService;

        public _TourDetailReviewsComponentPartial(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var values = await _reviewService.GetAllReviewAsync();
            var filtered = values.Where(x => x.TourId == tourId).ToList();
            return View(filtered);
        }
    }
}