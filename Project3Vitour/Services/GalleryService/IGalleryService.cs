using Project3Vitour.Dtos.GalleryDto;

namespace Project3Vitour.Services.GalleryService
{
    public interface IGalleryService
    {
        Task<List<ResultGalleryDto>> GetAllGalleryAsync();
        Task CreateGalleryAsync(CreateGalleryDto createGalleryDto);
        Task UpdateGalleryAsync(UpdateGalleryDto updateGalleryDto);
        Task<GetGalleryByIdDto> GetGalleryByIdAsync(string id);
        Task DeleteGalleryAsync(string id);

    }
}
