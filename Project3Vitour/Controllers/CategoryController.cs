using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.CategoryDto;
using Project3Vitour.Services.CatregoryService;

namespace Project3Vitour.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            createCategoryDto.CategoryStatus = true;
            await _categoryService.CreateCategoryAsync(createCategoryDto);
            return View(createCategoryDto);
        }
    }
}
