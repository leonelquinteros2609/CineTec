using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class SalaConfiguration : IEntityTypeConfiguration<Sala>
    {
        public void Configure(EntityTypeBuilder<Sala> builder)
        {
            builder.ToTable("salas");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            builder.Property(e => e.Filas).HasColumnName("filas").IsRequired();
            builder.Property(e => e.Columnas).HasColumnName("columnas").IsRequired();
            builder.Property(e => e.Capacidad).HasColumnName("capacidad").IsRequired();
        }
    }
}
