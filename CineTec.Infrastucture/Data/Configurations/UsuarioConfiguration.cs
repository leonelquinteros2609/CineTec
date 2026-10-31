using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuarios");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            builder.Property(e => e.Rol).HasColumnName("rol").HasMaxLength(20).HasDefaultValue("Cliente");
            builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

            builder.Ignore(e => e.Correo);
        }
    }
}
