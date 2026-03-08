using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.GalleryService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailShotGalleryComponentPartial : ViewComponent
    {
        private readonly IGalleryService _galleryService;

        public _TourDetailShotGalleryComponentPartial(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var values = await _galleryService.GetAllGalleryAsync();
            var filtered = values.Where(x => x.TourId == tourId).ToList();
            return View(filtered);
        }
    }
}
