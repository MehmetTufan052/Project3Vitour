using Microsoft.AspNetCore.Mvc;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourBreadCrumdComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }

}
