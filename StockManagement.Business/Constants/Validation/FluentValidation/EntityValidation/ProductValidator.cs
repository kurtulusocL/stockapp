using FluentValidation;
using StockManagement.Domain.Entities;

namespace StockManagement.Business.Constants.Validation.FluentValidation.EntityValidation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(i => i.Name).NotEmpty().WithMessage("product name can not be null");
            RuleFor(i => i.Code).NotEmpty().Length(6).WithMessage("product code can not be null and lenght must be 6 characters");
            RuleFor(i => i.Description).NotEmpty().WithMessage("description can not be null");
            RuleFor(i => i.Price).NotEmpty().GreaterThan(0).WithMessage("price can not be null and must be greater then 0");
            RuleFor(i => i.ImageUrl).NotEmpty().WithMessage("image url can not be null");
            RuleFor(i => i.CategoryId).NotEmpty().GreaterThan(0).WithMessage("CategoryId can not be null and must be greater then 0");
            RuleFor(i => i.AppUserId).NotEmpty().WithMessage("AppUserId can not be null");
            RuleFor(i => i.WarehouseId).NotEmpty().GreaterThan(0).WithMessage("WarehouseId can not be null and must be greater then 0");
        }
    }
}
