using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class CompraConfiguration : IEntityTypeConfiguration<Compra>
    {
        public void Configure(EntityTypeBuilder<Compra> builder)
        {
            builder.ToTable("compras");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.UsuarioId).HasColumnName("usuario_id").IsRequired();
            builder.Property(e => e.FuncionId).HasColumnName("funcion_id").IsRequired();
            builder.Property(e => e.FechaCompra).HasColumnName("fecha_compra").IsRequired();
            builder.Property(e => e.FechaExpiracion).HasColumnName("fecha_expiracion").IsRequired();
            builder.Property(e => e.Total).HasColumnName("total").HasPrecision(10, 2).IsRequired();
            builder.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).HasDefaultValue("Pendiente");

            builder.HasOne(d => d.Usuario)
                .WithMany(p => p.Compras)
                .HasForeignKey(d => d.UsuarioId);

            builder.HasOne(d => d.Funcion)
                .WithMany(p => p.Compras)
                .HasForeignKey(d => d.FuncionId);
        }
    }
}
