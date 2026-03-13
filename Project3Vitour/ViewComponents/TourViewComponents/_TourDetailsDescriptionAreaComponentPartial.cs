using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailsDescriptionAreaComponentPartial : ViewComponent
    {
        private readonly ITourService _tourService;

        public _TourDetailsDescriptionAreaComponentPartial(ITourService tourService)
        {
            _tourService = tourService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var value = await _tourService.GetTourByIdAsync(tourId);
            return View(value);
        }
    }
}
