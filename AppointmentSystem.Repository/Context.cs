using AppointmentSystem.Entity.Entity;
using AppointmentSystem.Utils.Converters;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Repository
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            base.ConfigureConventions(builder);

            builder.Properties<DateOnly>()
           .HaveConversion<DateOnlyConverter>();
        }

    }
}
