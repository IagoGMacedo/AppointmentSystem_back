using AppointmentSystem.Repository;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Api.Configuration
{
    public static class DataBaseConfiguration
    {
        public static void AddDataBaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Context>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
