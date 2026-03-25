using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.GalleryService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourFooterComponentPartial:ViewComponent
    {
        private readonly IGalleryService _galleryService;

        public _TourFooterComponentPartial(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var galleryList=await _galleryService.GetAllGalleryAsync();
            var top6 = galleryList.Take(6).ToList();
            return View(top6);
        }
    }
}
