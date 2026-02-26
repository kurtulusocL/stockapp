using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.UserSessionDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class UserSessionMappingProfile : Profile
    {
        public UserSessionMappingProfile()
        {
            CreateMap<UserSession, UserSessionGetDto>();
            CreateMap<UserSessionCreateDto, UserSession>();
        }
    }
}
