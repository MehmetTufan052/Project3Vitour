using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.GalleryService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailDestinationSmallGalleryComponentPartial:ViewComponent
    {
        private readonly IGalleryService _galleryService;

        public _TourDetailDestinationSmallGalleryComponentPartial(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _galleryService.GetAllGalleryAsync();
            return View(values);
          
        }
    }
}
