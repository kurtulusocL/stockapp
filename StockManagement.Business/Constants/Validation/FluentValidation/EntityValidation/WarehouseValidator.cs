using FluentValidation;
using StockManagement.Domain.Entities;

namespace StockManagement.Business.Constants.Validation.FluentValidation.EntityValidation
{
    public class WarehouseValidator : AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            RuleFor(i => i.Name).NotEmpty().WithMessage("name can not be null");
            RuleFor(i => i.Code).NotEmpty().Length(6).WithMessage("code can not be null and must be 6 character");
            RuleFor(i => i.Address).NotEmpty().WithMessage("address can not be null");
            RuleFor(i => i.TypeOfWarehouse).NotEmpty().WithMessage("type of warehouse can not be null");
        }
    }
}
