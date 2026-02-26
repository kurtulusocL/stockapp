using FluentValidation;
using StockManagement.Shared.ViewModels.RoleVM;

namespace StockManagement.Business.Constants.Validation.FluentValidation.ViewModelValidation
{
    public class RoleVMValidation : AbstractValidator<RoleAssignVM>
    {
        public RoleVMValidation()
        {
            RuleFor(i => i.RoleId).NotEmpty().WithMessage("roleId can not be null");
            RuleFor(i => i.RoleName).NotEmpty().WithMessage("role name can not be null");
            RuleFor(i => i.HasAssign).NotEmpty().WithMessage("has assign can not be null");
        }
    }
}
