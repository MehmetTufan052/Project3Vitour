using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.TourPlanService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailLast3TourComponentPartial : ViewComponent
    {
        ITourService _tourService;

        public _TourDetailLast3TourComponentPartial(ITourService tourService)
        {
            _tourService = tourService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _tourService.GetAllTourAsync();
            var last3 = values.TakeLast(3).ToList();
            return View(last3);
        }
    }
}