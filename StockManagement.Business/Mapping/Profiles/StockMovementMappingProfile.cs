using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.StockMovementDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class StockMovementMappingProfile:Profile
    {
        public StockMovementMappingProfile()
        {
            CreateMap<StockMovement, StockMovementGetDto>();
        }
    }
}
