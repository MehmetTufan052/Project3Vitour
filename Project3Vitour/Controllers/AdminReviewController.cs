using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Models;
using Project3Vitour.Services.ReviewService;

namespace Project3Vitour.Controllers
{
    public class AdminReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public AdminReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _reviewService.GetAllReviewAsync();
            return View(values);
        }
        public async Task<IActionResult> DeleteReview(string id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SentimentReport(string tourId)
        {
            var vm = new SentimentViewModel
            {
                Ozet = await _reviewService.GetSentimentSummaryAsync(tourId),
                Trend = await _reviewService.GetMonthlyTrendAsync(tourId),
                Yorumlar = await _reviewService.GetAllReviewsByTourIdAsync(tourId)
            };

            return View(vm);
        }
    }
}
