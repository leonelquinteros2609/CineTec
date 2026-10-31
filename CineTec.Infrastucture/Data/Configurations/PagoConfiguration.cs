using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class PagoConfiguration : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.ToTable("pagos");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.CompraId).HasColumnName("compra_id").IsRequired();
            builder.Property(e => e.Monto).HasColumnName("monto").HasPrecision(10, 2).IsRequired();
            builder.Property(e => e.MetodoPago).HasColumnName("metodo_pago").HasMaxLength(30).IsRequired();
            builder.Property(e => e.FechaPago).HasColumnName("fecha_pago").IsRequired();
            builder.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("Completado");

            builder.HasOne(d => d.Compra)
                .WithMany(p => p.Pagos)
                .HasForeignKey(d => d.CompraId);
        }
    }
}
