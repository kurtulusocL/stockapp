using FluentValidation;
using StockManagement.Business.Constants.Helpers;
using StockManagement.Shared.Dtos.AuthDtos;

namespace StockManagement.Business.Constants.Validation.FluentValidation.DtoValidation
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(i => i.Email).EmailAddress().NotEmpty().WithMessage("email address can not be null and must be valid email");
            RuleFor(i => i.PhoneNumber)
                 .Must(PhoneNumberValidationHelpers.IsNumeric)
                .Must(PhoneNumberValidationHelpers.IsValidPhone).NotEmpty().WithMessage("phone number can not be null");
        }
    }
}
