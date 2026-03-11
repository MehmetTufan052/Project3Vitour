using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.ReviewDto;
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

        [HttpPost]
        public async Task<IActionResult> AnalyzeAll()
        {
            var values = await _reviewService.AnalyzeAllReviewSentimentsAsync();
            return Json(values.Select(ToClientReview));
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var value = await _reviewService.AnalyzeReviewSentimentAsync(id);
            if (value == null)
            {
                return NotFound();
            }

            return Json(ToClientReview(value));
        }

        [HttpGet]
        public async Task<IActionResult> DebugCount()
        {
            var values = await _reviewService.GetAllReviewAsync();
            return Json(new
            {
                count = values.Count,
                sample = values.Take(3).Select(x => new { x.ReviewId, x.NameSurname, x.SentimentLabel })
            });
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

        private static object ToClientReview(ResultReviewDto review)
        {
            return new
            {
                id = review.ReviewId ?? string.Empty,
                name = review.NameSurname ?? string.Empty,
                initials = !string.IsNullOrWhiteSpace(review.NameSurname) ? review.NameSurname.Trim()[0].ToString().ToUpper() : "?",
                gradient = "linear-gradient(135deg,#3b82f6,#6366f1)",
                date = review.ReviewDate == default ? string.Empty : review.ReviewDate.ToString("dd MMM yyyy"),
                text = review.Detail ?? string.Empty,
                stars = review.Score,
                sentiment = string.IsNullOrWhiteSpace(review.SentimentLabel) ? "Nötr" : review.SentimentLabel,
                score = CalculateDisplayScore(review.SentimentLabel, review.SentimentScore),
                status = review.Status ? "Aktif" : "Pasif"
            };
        }

        private static int CalculateDisplayScore(string? sentimentLabel, double confidence)
        {
            var clampedConfidence = Math.Clamp(confidence, 0d, 1d);
            var normalized = (sentimentLabel ?? string.Empty).Trim().ToLowerInvariant();

            return normalized switch
            {
                "olumlu" => (int)Math.Round(65 + (clampedConfidence * 35)),
                "olumsuz" => (int)Math.Round(Math.Max(0, 50 - (clampedConfidence * 50))),
                "nötr" or "notr" or "neutral" => (int)Math.Round(50 + (clampedConfidence * 15)),
                _ => (int)Math.Round(50 + (clampedConfidence * 15))
            };
        }
    }
}
