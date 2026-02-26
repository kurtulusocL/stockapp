using FluentValidation;
using StockManagement.Business.Constants.Helpers;
using StockManagement.Shared.Dtos.AuthDtos;

namespace StockManagement.Business.Constants.Validation.FluentValidation.DtoValidation
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(i => i.NameSurname).NotEmpty().WithMessage("name surname can not be null");
            RuleFor(i => i.Email).EmailAddress().NotEmpty().WithMessage("email address can not be null and must be valid email");
            RuleFor(i => i.Title).NotEmpty().WithMessage("name surname can not be null");
            RuleFor(i => i.PhoneNumber)
                .Must(PhoneNumberValidationHelpers.IsNumeric)
                .Must(PhoneNumberValidationHelpers.IsValidPhone).NotEmpty().WithMessage("phone number can not be null");
            RuleFor(i => i.Password).MinimumLength(8).NotEmpty().WithMessage("password can not be null and must be min 8 characters");
            RuleFor(i => i.ConfirmPassword).MinimumLength(8).Equal(i => i.ConfirmPassword).NotEmpty().WithMessage("confirmed new password can not be null, must be same with password and min 8 characters");
        }
    }
}
