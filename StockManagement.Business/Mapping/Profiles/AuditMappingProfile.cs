using AutoMapper;
using StockManagement.Domain.Entities;
using StockManagement.Shared.Dtos.MappingDtos.AuditDtos;

namespace StockManagement.Business.Mapping.Profiles
{
    public class AuditMappingProfile:Profile
    {
        public AuditMappingProfile()
        {
            CreateMap<Audit, AuditGetDto>();
        }
    }
}
