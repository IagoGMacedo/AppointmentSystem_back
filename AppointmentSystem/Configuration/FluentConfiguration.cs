using AppointmentSystem.Validator;
using FluentValidation.AspNetCore;

namespace AppointmentSystem.Api.Configuration
{
    public static class FluentConfiguration
    {
        public static void AddFluentConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<AppointmentRegistrationValidator>());
            services.AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<UserRegistrationValidator>());
        }
    }
}
