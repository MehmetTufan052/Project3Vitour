using Project3Vitour.Dtos.CategoryDto;

namespace Project3Vitour.Services.CatregoryService
{
    public interface ICategoryService
    {
        Task<List<ResultCategoryDto>> GetAllCategoryAsync();
        Task CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(string id);

        Task<GetCategoryByIdDto> GetCategoryByIdAsync(string id);
    }
}
