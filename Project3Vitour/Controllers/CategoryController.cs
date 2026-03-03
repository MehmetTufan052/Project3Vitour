using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.CategoryDto;
using Project3Vitour.Services.CatregoryService;

namespace Project3Vitour.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> CategoryList()
        {
            var values = await _categoryService.GetAllCategoryAsync();
            return View(values);
        }

        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction("CategoryList");
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

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(string id)
        {
            var value = await _categoryService.GetCategoryByIdAsync(id);
            var dto = _mapper.Map<UpdateCategoryDto>(value);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto dto)
        {
            await _categoryService.UpdateCategoryAsync(dto);
            return RedirectToAction("CategoryList");
        }
    }
}
