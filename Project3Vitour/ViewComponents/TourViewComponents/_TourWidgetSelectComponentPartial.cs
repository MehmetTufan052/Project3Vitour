using Microsoft.AspNetCore.Mvc;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourWidgetSelectComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
