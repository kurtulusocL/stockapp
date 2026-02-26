using FluentValidation;
using StockManagement.Shared.Dtos.AuthDtos;

namespace StockManagement.Business.Constants.Validation.FluentValidation.DtoValidation
{
    public class ConfirmCodeDtoValidator : AbstractValidator<ConfirmCodeDto>
    {
        public ConfirmCodeDtoValidator()
        {
            RuleFor(i => i.Email).EmailAddress().NotEmpty().WithMessage("email address can not be null and must be valid email");
            RuleFor(i => i.ConfirmCode).NotEmpty().WithMessage("code can not be null");
        }
    }
}
