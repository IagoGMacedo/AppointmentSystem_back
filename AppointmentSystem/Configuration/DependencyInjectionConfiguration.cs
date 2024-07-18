using AppointmentSystem.Api.Middleware;
using AppointmentSystem.Business.Business;
using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Repository;
using AppointmentSystem.Repository.Interface.IRepository;
using AppointmentSystem.Repository.Repository;
using AppointmentSystem.Utils.Configurations;
using AppointmentSystem.Utils.UserContext;

namespace AppointmentSystem.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            InjectRepository(services);
            InjectBusiness(services);
            InjectMiddleware(services);

            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddOptions<AuthenticationConfig>().Bind(configuration.GetSection("Authorization"));
        }

        private static void InjectRepository(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        }

        private static void InjectBusiness(IServiceCollection services)
        {
            services.AddScoped<IAppointmentBusiness, AppointmentBusiness>();
            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddScoped<IAuthenticationBusiness, AuthenticationBusiness>();
        }

        private static void InjectMiddleware(IServiceCollection services)
        {
            services.AddTransient<ApiMiddleware>();
            services.AddTransient<UserContextMiddleware>();
        }
    }
}
