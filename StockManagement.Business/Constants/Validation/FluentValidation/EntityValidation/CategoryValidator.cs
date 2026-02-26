using FluentValidation;
using StockManagement.Domain.Entities;

namespace StockManagement.Business.Constants.Validation.FluentValidation.EntityValidation
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(i => i.Name).NotEmpty().WithMessage("category name can not be null");
            RuleFor(i => i.Code).NotEmpty().Length(3).WithMessage("category code can not be null and lenght must 3 characters");
        }
    }
}
