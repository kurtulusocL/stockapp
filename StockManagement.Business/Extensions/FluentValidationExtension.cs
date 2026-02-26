using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StockManagement.Business.Constants.Validation.FluentValidation.DtoValidation;
using StockManagement.Business.Constants.Validation.FluentValidation.EntityValidation;
using StockManagement.Business.Constants.Validation.FluentValidation.ViewModelValidation;

namespace StockManagement.Business.Extensions
{
    public static class FluentValidationExtension
    {
        public static void AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AppRoleValidator>();
            services.AddValidatorsFromAssemblyContaining<AppUserValidator>();
            services.AddValidatorsFromAssemblyContaining<CategoryValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductValidator>();
            services.AddValidatorsFromAssemblyContaining<UnitInStockValidator>();
            services.AddValidatorsFromAssemblyContaining<WarehouseValidator>();

            services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ConfirmCodeDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ForgotPasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginConfirmCodeDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<ResetPasswordDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateProfileDtoValidator>();

            services.AddValidatorsFromAssemblyContaining<RoleVMValidation>();
        }
    }
}
