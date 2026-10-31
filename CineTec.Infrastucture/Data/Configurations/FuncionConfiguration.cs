using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class FuncionConfiguration : IEntityTypeConfiguration<Funcion>
    {
        public void Configure(EntityTypeBuilder<Funcion> builder)
        {
            builder.ToTable("funciones");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.PeliculaId).HasColumnName("pelicula_id").IsRequired();
            builder.Property(e => e.SalaId).HasColumnName("sala_id").IsRequired();
            builder.Property(e => e.FechaHoraInicio).HasColumnName("fecha_hora_inicio").IsRequired();
            builder.Property(e => e.FechaHoraFin).HasColumnName("fecha_hora_fin").IsRequired();
            builder.Property(e => e.PrecioBase).HasColumnName("precio_base").HasPrecision(10, 2).IsRequired();
            builder.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("Activa");

            builder.HasOne(d => d.Pelicula)
                .WithMany(p => p.Funciones)
                .HasForeignKey(d => d.PeliculaId);

            builder.HasOne(d => d.Sala)
                .WithMany(p => p.Funciones)
                .HasForeignKey(d => d.SalaId);
        }
    }
}
