using AppointmentSystem.Entity.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Repository.Map
{
    public class AppointmentMap : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {

            builder.ToTable("tb_agendamento");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id_agendamento")
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasColumnName("id_user")
                .IsRequired();

            builder.Property(e => e.AppointmentDate)
                .HasColumnName("dat_agendamento")
                .IsRequired();

            builder.Property(e => e.AppointmentTime)
                .HasColumnName("hor_agendamento")
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnName("dsc_status")
                .IsRequired();

            builder.Property(e => e.DateOfCreation)
                .HasColumnName("dat_criacao")
                .IsRequired();

            builder.HasOne(e => e.User)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

        }
    }
}
