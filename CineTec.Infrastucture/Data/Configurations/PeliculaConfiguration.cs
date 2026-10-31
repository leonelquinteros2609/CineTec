using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class PeliculaConfiguration : IEntityTypeConfiguration<Pelicula>
    {
        public void Configure(EntityTypeBuilder<Pelicula> builder)
        {
            builder.ToTable("peliculas");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(150).IsRequired();
            builder.Property(e => e.Sinopsis).HasColumnName("sinopsis").HasColumnType("text");
            builder.Property(e => e.DuracionMin).HasColumnName("duracion_min").IsRequired();
            builder.Property(e => e.Clasificacion).HasColumnName("clasificacion").HasMaxLength(20).IsRequired();
            builder.Property(e => e.Genero).HasColumnName("genero").HasMaxLength(50).IsRequired();
            builder.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("Activa");

            builder.Ignore(e => e.DuracionMinutos);
        }
    }
}
