using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.AppRoleDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class AppRoleMappingProfile : Profile
    {
        public AppRoleMappingProfile()
        {
            CreateMap<AppRole, AppRoleGetDto>();
            CreateMap<AppRoleCreateDto, AppRole>();
            CreateMap<AppRole, AppRoleUpdateDto>();
            CreateMap<AppRoleUpdateDto, AppRole>().ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.Now.ToLocalTime()));
        }
    }
}
