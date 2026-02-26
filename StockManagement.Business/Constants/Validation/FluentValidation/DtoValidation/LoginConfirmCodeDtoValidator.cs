using FluentValidation;
using StockManagement.Shared.Dtos.AuthDtos;

namespace StockManagement.Business.Constants.Validation.FluentValidation.DtoValidation
{
    public class LoginConfirmCodeDtoValidator : AbstractValidator<LoginConfirmCodeDto>
    {
        public LoginConfirmCodeDtoValidator()
        {
            RuleFor(i => i.Email).EmailAddress().NotEmpty().WithMessage("email address can not be null and must be valid email");
            RuleFor(i => i.LoginConfirmCode).NotEmpty().WithMessage("confirm code can not be null");
        }
    }
}
