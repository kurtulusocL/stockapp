using FluentValidation;
using StockManagement.Shared.Dtos.AuthDtos;

namespace StockManagement.Business.Constants.Validation.FluentValidation.DtoValidation
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(i => i.Email).EmailAddress().NotEmpty().WithMessage("email address can not be null and must be valid email");
            RuleFor(i => i.NewPassword).MinimumLength(8).NotEmpty().WithMessage("new password can not be null, must be min 8 characters");
            RuleFor(i => i.ConfirmNewPassword).MinimumLength(8).Equal(i => i.NewPassword).NotEmpty().WithMessage("confirmed new password can not be null,, must be same with new password and min 8 characters");
        }
    }
}
