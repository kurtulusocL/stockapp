using FluentValidation;
using StockManagement.Domain.Entities;

namespace StockManagement.Business.Constants.Validation.FluentValidation.EntityValidation
{
    public class UnitInStockValidator: AbstractValidator<UnitInStock>
    {
        public UnitInStockValidator()
        {
            RuleFor(i => i.Quantity).NotEmpty().WithMessage("quantity can not be null");
            RuleFor(i => i.Code).NotEmpty().Length(6).WithMessage("quantity can not be null and must be 6 characters");
            RuleFor(i => i.WarehouseId).NotEmpty().GreaterThan(0).WithMessage("WarehouseId can not be null and must be greater then 0");
        }
    }
}
