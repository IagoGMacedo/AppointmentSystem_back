using AppointmentSystem.Api.Middleware;
using AppointmentSystem.Repository;
using AppointmentSystem.Repository.Interface.IRepository;

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
        }

        private static void InjectBusiness(IServiceCollection services)
        {
        }

        private static void InjectMiddleware(IServiceCollection services)
        {
            services.AddTransient<ApiMiddleware>();
        }
    }
}
