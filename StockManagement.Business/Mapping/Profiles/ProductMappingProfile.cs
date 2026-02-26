using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.ProductDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductGetDto>()
                .ForMember(dest => dest.StockMovementCount, opt => opt.MapFrom(src => src.StockMovements != null ? src.StockMovements.Count : 0))
                .ForMember(dest => dest.UnitInStockCount, opt => opt.MapFrom(src => src.UnitInStocks != null ? src.UnitInStocks.Count : 0));

            CreateMap<ProductCreateDto, Product>();
            CreateMap<Product, ProductUpdateDto>();

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()))
                .ForMember(dest => dest.StockMovements, opt => opt.Ignore())
                .ForMember(dest => dest.UnitInStocks, opt => opt.Ignore());
        }
    }
}
