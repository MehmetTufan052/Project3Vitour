using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Services.TourServices.ITourService;

namespace Project3Vitour.Controllers
{
    public class AdminTourController : Controller
    {
        private readonly ITourService _tourService;
        private readonly IMapper _mapper;

        public AdminTourController(ITourService tourService, IMapper mapper)
        {
            _tourService = tourService;
            _mapper = mapper;
        }

        public async Task<IActionResult> TourList()
        {
            var values = await _tourService.GetAllTourAsync();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateTour()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTour(CreateTourDto createTourDto)
        {
            await _tourService.CreateTourAsync(createTourDto);
            return RedirectToAction("TourList");
        }

        public async Task<IActionResult> DeleteTour(string id)
        {
            await _tourService.DeleteTourAsync(id);
            return RedirectToAction("TourList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTour(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("TourList");
            }

            var value = await _tourService.GetTourByIdAsync(id);
            if (value == null)
            {
                return NotFound("Güncellenecek tur bulunamadı.");
            }

            var updateDto = _mapper.Map<UpdateTourDto>(value);
            if (updateDto == null)
            {
                return NotFound("Tur verisi güncelleme modeline dönüştürülemedi.");
            }

            return View(updateDto);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateTour(UpdateTourDto updateTourDto)
        {
            await _tourService.UpdateTourAsync(updateTourDto);
            return RedirectToAction("TourList");
        }
    }
}


