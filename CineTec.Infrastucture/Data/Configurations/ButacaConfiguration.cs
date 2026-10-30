using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data.Configurations
{
    public class ButacaConfiguration : IEntityTypeConfiguration<Butaca>
    {
        public void Configure(EntityTypeBuilder<Butaca> builder)
        {
            builder.ToTable("butacas");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.SalaId).HasColumnName("sala_id").IsRequired();
            builder.Property(e => e.Fila).HasColumnName("fila").HasMaxLength(5).IsRequired();
            builder.Property(e => e.Columna).HasColumnName("columna").IsRequired();
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();

            builder.HasOne(d => d.Sala)
                .WithMany(p => p.Butacas)
                .HasForeignKey(d => d.SalaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
