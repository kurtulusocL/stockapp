using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.AppUserDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class AppUserMappingProfile:Profile
    {
        public AppUserMappingProfile()
        {
            CreateMap<AppUser, AppUserGetDto>()
                .ForMember(dest => dest.AuditCount, opt => opt.MapFrom(src => src.Audits != null ? src.Audits.Count : 0))
                .ForMember(dest => dest.StockMovementCount, opt => opt.MapFrom(src => src.StockMovements != null ? src.StockMovements.Count : 0))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
                .ForMember(dest => dest.UnitInStockCount, opt => opt.MapFrom(src => src.UnitInStocks != null ? src.UnitInStocks.Count : 0))
                .ForMember(dest => dest.UserSessionCount, opt => opt.MapFrom(src => src.UserSessions != null ? src.UserSessions.Count : 0));
        }
    }
}
