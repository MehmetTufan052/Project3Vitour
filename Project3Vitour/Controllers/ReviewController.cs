using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Helpers;
using Project3Vitour.Services.ReservationService;
using Project3Vitour.Services.ReviewService;
using Project3Vitour.Services.TranslationService;
using Project3Vitour.Settings;
using System.Threading.Tasks;

namespace Project3Vitour.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IReviewService _reviewService;
        private readonly IReservationService _reservationService;
        private readonly ITranslationService _translationService;
        private readonly bool _requireReservationEmailMatch;

        public ReviewController(IMapper mapper, IReviewService reviewService, IReservationService reservationService, ITranslationService translationService, IOptions<ReviewSettings> reviewSettings)
        {
            _mapper = mapper;
            _reviewService = reviewService;
            _reservationService = reservationService;
            _translationService = translationService;
            _requireReservationEmailMatch = reviewSettings.Value.RequireReservationEmailMatch;
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
            return RedirectToAction("GetReviewByTourId", new { id = createReviewDto.TourId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromTour(CreateReviewFormDto model)
        {
            if (string.IsNullOrWhiteSpace(model?.TourId))
                return RedirectToAction("TourList", "Tour");

            if (string.IsNullOrWhiteSpace(model.NameSurname) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Detail) ||
                model.ValueForMoneyScore < 1 || model.ValueForMoneyScore > 5 ||
                model.DestinationScore < 1 || model.DestinationScore > 5 ||
                model.AccommodationScore < 1 || model.AccommodationScore > 5 ||
                model.TransportScore < 1 || model.TransportScore > 5 ||
                !model.AcceptTerms)
            {
                TempData["ReviewError"] = "Lütfen tüm alanları doğru doldurun, yıldız puanı verin ve koşulları kabul edin.";
                return RedirectToAction("TourDetails", "Tour", new { id = model.TourId });
            }

            if (_requireReservationEmailMatch)
            {
                var hasReservation = await _reservationService.ExistsReservationForTourAndEmailAsync(model.TourId, model.Email);
                if (!hasReservation)
                {
                    TempData["ReviewError"] = "Bu tur için yalnızca rezervasyonda kayıtlı e-posta ile yorum yapabilirsiniz.";
                    return RedirectToAction("TourDetails", "Tour", new { id = model.TourId });
                }
            }

            var createReviewDto = _mapper.Map<CreateReviewDto>(model);
            await _reviewService.CreateReviewAsync(createReviewDto);
            TempData["ReviewSuccess"] = "Yorumunuz başarıyla kaydedildi.";

            return RedirectToAction("TourDetails", "Tour", new { id = model.TourId });
        }

        public async Task<IActionResult> GetReviewByTourId(string id)
        {
            var values = await _reviewService.GetAllReviewsByTourIdAsync(id);
            values = await _translationService.LocalizeReviewsByTourAsync(values, RequestLanguageHelper.GetCurrentLanguage(HttpContext));
            return View(values);
        }

        public async Task<IActionResult> DeleteReview(string id)
        {
            await _reviewService.GetReviewByIdAsync(id);
            return RedirectToAction("Index");
        }
    }
}





