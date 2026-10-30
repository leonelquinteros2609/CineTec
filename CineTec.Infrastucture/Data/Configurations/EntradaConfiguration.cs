using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class EntradaConfiguration : IEntityTypeConfiguration<Entrada>
    {
        public void Configure(EntityTypeBuilder<Entrada> builder)
        {
            builder.ToTable("entradas");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.CompraId).HasColumnName("compra_id").IsRequired();
            builder.Property(e => e.FuncionId).HasColumnName("funcion_id").IsRequired();
            builder.Property(e => e.ButacaId).HasColumnName("butaca_id").IsRequired();
            builder.Property(e => e.Precio).HasColumnName("precio").HasPrecision(10, 2).IsRequired();
            builder.Property(e => e.CodigoUnico).HasColumnName("codigo_unico").HasMaxLength(64);
            builder.HasIndex(e => e.CodigoUnico).IsUnique();
            builder.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("Reservada");

            builder.HasOne(d => d.Compra)
                .WithMany(p => p.Entradas)
                .HasForeignKey(d => d.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Funcion)
                .WithMany(p => p.Entradas)
                .HasForeignKey(d => d.FuncionId);

            builder.HasOne(d => d.Butaca)
                .WithMany(p => p.Entradas)
                .HasForeignKey(d => d.ButacaId);
        }
    }
}
