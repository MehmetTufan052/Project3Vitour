using AutoMapper;
using Project3Vitour.Dtos.CategoryDto;
using Project3Vitour.Dtos.GalleryDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Dtos.TourPlanDto;
using Project3Vitour.Entities;

namespace Project3Vitour.Mapping
{
    public class GeneralMapping:Profile
    {
        public GeneralMapping()
        {
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, ResultCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();
            CreateMap<Category, GetCategoryByIdDto>().ReverseMap();

            CreateMap<Tour, CreateTourDto>().ReverseMap();
            CreateMap<Tour, ResultTourDto>().ReverseMap();
            CreateMap<Tour, UpdateTourDto>().ReverseMap();
            CreateMap<Tour, GetTourByIdDto>().ReverseMap();
            CreateMap<ResultTourDto, UpdateTourDto>().ReverseMap();
            CreateMap<GetTourByIdDto, UpdateTourDto>().ReverseMap();
            CreateMap<GetTourByIdDto, ResultTourDto>();

            CreateMap<Review, CreateReviewDto>().ReverseMap();
            CreateMap<Review, UpdateReviewDto>().ReverseMap();
            CreateMap<Review, GetReviewByIdDto>().ReverseMap();
            CreateMap<Review, ResultReviewDto>().ReverseMap();
            CreateMap<Review, ResultReviewByTourIdDto>().ReverseMap();
            CreateMap<CreateReviewFormDto, CreateReviewDto>()
                .ForMember(d => d.NameSurname, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.NameSurname) ? s.NameSurname : s.NameSurname.Trim()))
                .ForMember(d => d.Detail, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Detail) ? s.Detail : s.Detail.Trim()))
                .ForMember(d => d.Score, o => o.MapFrom(s => (int)Math.Round((s.ValueForMoneyScore + s.DestinationScore + s.AccommodationScore + s.TransportScore) / 4.0)))
                .ForMember(d => d.ReviewDate, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.Status, o => o.MapFrom(_ => true));

            CreateMap<Gallery, CreateGalleryDto>().ReverseMap();
            CreateMap<Gallery, UpdateGalleryDto>().ReverseMap();
            CreateMap<Gallery, GetGalleryByIdDto>().ReverseMap();
            CreateMap<Gallery, ResultGalleryDto>().ReverseMap();

            CreateMap<TourPlan, CreateTourPlanDto>().ReverseMap();
            CreateMap<TourPlan, UpdateTourPlanDto>().ReverseMap();
            CreateMap<TourPlan, ResultTourPlanDto>().ReverseMap();
            CreateMap<TourPlan, GetTourPlanByIdDto>().ReverseMap();
            CreateMap<GetCategoryByIdDto, UpdateCategoryDto>().ReverseMap();
        }
        
    }
}





