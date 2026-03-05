using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Entities;
using Project3Vitour.Services.TourPlanService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailTourPlaningComponentPartial : ViewComponent
    {
        ITourPlanService _tourPlanService;

        public _TourDetailTourPlaningComponentPartial(ITourPlanService tourPlanService)
        {
            _tourPlanService = tourPlanService;
        }

        public async Task< IViewComponentResult> InvokeAsync(string tourId)
        {
            var values = await _tourPlanService.GetAllTourPlanAsync();
            var filtered = values.Where(x => x.TourId == tourId).ToList();
            return View(filtered);
        }
    }
}
