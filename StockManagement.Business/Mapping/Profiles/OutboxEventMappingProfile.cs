using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.OutboxEventDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class OutboxEventMappingProfile : Profile
    {
        public OutboxEventMappingProfile()
        {
            CreateMap<OutboxEvent, OutboxEventGetDto>();
        }
    }
}
