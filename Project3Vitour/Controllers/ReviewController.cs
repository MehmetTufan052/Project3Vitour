using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Services.ReviewService;

namespace Project3Vitour.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public IActionResult CreateReview()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDto createReviewDto)
        {
            createReviewDto.Status = false;
            await _reviewService.CreateReviewAsync(createReviewDto);
            return RedirectToAction("ReviewList");
        }
        public async Task<IActionResult> GetReviewByTourId(string id)
        {
            var values = await _reviewService.GetAllReviewsByTourIdAsync(id);
            return View(values);
        }
    }
}
