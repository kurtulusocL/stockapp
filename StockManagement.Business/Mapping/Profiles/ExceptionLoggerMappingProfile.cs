using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.ExceptionLoggerDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class ExceptionLoggerMappingProfile:Profile
    {
        public ExceptionLoggerMappingProfile()
        {
            CreateMap<ExceptionLogger, ExceptionLoggerGetDto>();
        }
    }
}
