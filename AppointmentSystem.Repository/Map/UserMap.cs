using AppointmentSystem.Entity.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Repository.Map
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("tb_paciente");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(e => e.Name)
                .HasColumnName("dsc_nome")
                .IsRequired();

            builder.Property(e => e.Login)
                .HasColumnName("lgn_usuario")
                .HasMaxLength(50)
                .IsRequired(true);

            builder.Property(e => e.Profile)
                .HasColumnName("id_tpperfil")
                .IsRequired(true);

            builder.Property(e => e.DateOfBirth)
                .HasColumnName("dat_nascimento")
                .IsRequired();

            builder.Property(e => e.DateOfCreation)
                .HasColumnName("dat_criacao")
                .IsRequired();

            builder.HasMany(e => e.Appointments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

        }
    }
}
