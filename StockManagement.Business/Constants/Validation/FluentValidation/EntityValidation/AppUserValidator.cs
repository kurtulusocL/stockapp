using FluentValidation;
using StockManagement.Domain.Entities;

namespace StockManagement.Business.Constants.Validation.FluentValidation.EntityValidation
{
    public class AppUserValidator: AbstractValidator<AppUser>
    {
        public AppUserValidator()
        {
            RuleFor(i => i.NameSurname).NotEmpty().WithMessage("name surame can not be null");
            RuleFor(i => i.Email).EmailAddress().NotEmpty().WithMessage("email can not be null");
            RuleFor(i => i.Title).NotEmpty().WithMessage("title can not be null");
            RuleFor(i => i.PhoneNumber).NotEmpty().WithMessage("phone number can not be null");
        }
    }
}
