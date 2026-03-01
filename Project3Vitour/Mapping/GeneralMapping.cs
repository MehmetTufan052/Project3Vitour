using AutoMapper;
using Project3Vitour.Dtos.CategoryDto;
using Project3Vitour.Dtos.ReviewDto;
using Project3Vitour.Dtos.TourDtos;
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

            CreateMap<Review, CreateReviewDto>().ReverseMap();
            CreateMap<Review, UpdateReviewDto>().ReverseMap();
            CreateMap<Review, GetReviewByIdDto>().ReverseMap();
            CreateMap<Review, ResultReviewDto>().ReverseMap();
            CreateMap<Review, ResultReviewByTourIdDto>().ReverseMap();
        }
        
    }
}
