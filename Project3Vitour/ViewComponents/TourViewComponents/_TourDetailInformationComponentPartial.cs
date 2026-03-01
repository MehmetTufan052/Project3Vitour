using Microsoft.AspNetCore.Mvc;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailInformationComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
