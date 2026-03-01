using Microsoft.AspNetCore.Mvc;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourScriptsComponentPartial : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
