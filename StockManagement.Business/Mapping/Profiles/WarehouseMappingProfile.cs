using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.WarehouseDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class WarehouseMappingProfile : Profile
    {
        public WarehouseMappingProfile()
        {
            CreateMap<Warehouse, WarehouseGetDto>()
                .ForMember(i => i.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
                .ForMember(i => i.UnitInStockCount, opt => opt.MapFrom(src => src.UnitInStocks != null ? src.UnitInStocks.Count : 0));


            CreateMap<WarehouseCreateDto, Warehouse>();
            CreateMap<Warehouse, WarehouseUpdateDto>();

            CreateMap<WarehouseUpdateDto, Warehouse>()
               .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()))
               .ForMember(dest => dest.Products, opt => opt.Ignore())
               .ForMember(dest => dest.UnitInStocks, opt => opt.Ignore());
        }
    }
}
