using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Services.GalleryService;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.ViewComponents.TourViewComponents
{
    public class _TourDetailDestinationSmallGalleryComponentPartial : ViewComponent
    {
        private readonly IGalleryService _galleryService;
        private readonly ITourService _tourService;

        public _TourDetailDestinationSmallGalleryComponentPartial(IGalleryService galleryService, ITourService tourService)
        {
            _galleryService = galleryService;
            _tourService = tourService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tourId)
        {
            var values = await _galleryService.GetAllGalleryAsync();
            var filtered = values.Where(x => x.TourId == tourId).ToList();

            if (!filtered.Any())
            {
                var tour = await _tourService.GetTourByIdAsync(tourId);
                if (!string.IsNullOrWhiteSpace(tour?.Title))
                {
                    filtered = values
                        .Where(x => string.Equals(x.TourName, tour.Title, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            return View(filtered);
        }
    }
}
