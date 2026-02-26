using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.CategoryDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategoryGetDto>().ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));

            CreateMap<CategoryCreateDto, Category>();
            CreateMap<Category, CategoryUpdateDto>();

            CreateMap<CategoryUpdateDto, Category>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()))
                .ForMember(dest => dest.Products, opt => opt.Ignore());
        }
    }
}
