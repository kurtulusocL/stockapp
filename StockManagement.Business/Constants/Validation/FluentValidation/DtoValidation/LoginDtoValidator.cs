using FluentValidation;
using StockManagement.Shared.Dtos.AuthDtos;

namespace StockManagement.Business.Constants.Validation.FluentValidation.DtoValidation
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(i => i.Email).EmailAddress().NotEmpty().WithMessage("email address can not be null and must be valid email");
            RuleFor(i => i.Password).MinimumLength(8).NotEmpty().WithMessage("password can not be null and must be min 8 characters");
        }
    }
}
