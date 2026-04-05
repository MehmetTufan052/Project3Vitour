using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Dtos.TourPlanDto;

namespace Project3Vitour.Models
{
    public class TourDetailsViewModel
    {
        public GetTourByIdDto? Tour { get; set; }
        public List<ResultTourPlanDto> TourPlans { get; set; } = new();
    }
}
