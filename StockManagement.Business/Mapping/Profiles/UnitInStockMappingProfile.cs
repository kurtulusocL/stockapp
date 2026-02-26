using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.UnitInStockStos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class UnitInStockMappingProfile : Profile
    {
        public UnitInStockMappingProfile()
        {
            CreateMap<UnitInStock, UnitInStockGetDto>();
            CreateMap<UnitInStockCreateDto, UnitInStock>();
            CreateMap<UnitInStock, UnitInStockUpdateDto>();
            CreateMap<UnitInStockUpdateDto, UnitInStock>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));
        }
    }
}
