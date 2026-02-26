using FluentValidation;
using StockManagement.Domain.Entities;

namespace StockManagement.Business.Constants.Validation.FluentValidation.EntityValidation
{
    public class AppRoleValidator:AbstractValidator<AppRole>
    {
        public AppRoleValidator()
        {
            RuleFor(i => i.Name).NotEmpty().WithMessage("role name can not be null");
            RuleFor(i => i.NormalizedName).NotEmpty().WithMessage("normalized role name can not be null");
        }
    }
}
