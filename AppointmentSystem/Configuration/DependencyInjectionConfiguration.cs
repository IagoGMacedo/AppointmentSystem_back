using AppointmentSystem.Api.Middleware;
using AppointmentSystem.Business.Business;
using AppointmentSystem.Business.Interface.IBusiness;
using AppointmentSystem.Repository;
using AppointmentSystem.Repository.Interface.IRepository;
using AppointmentSystem.Repository.Repository;

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
        }

        private static void InjectRepository(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }

        private static void InjectBusiness(IServiceCollection services)
        {
            services.AddScoped<IUserBusiness, UserBusiness>();
        }

        private static void InjectMiddleware(IServiceCollection services)
        {
            services.AddTransient<ApiMiddleware>();
        }
    }
}
