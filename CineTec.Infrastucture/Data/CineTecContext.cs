using Microsoft.EntityFrameworkCore;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data;

public partial class CineTecContext : DbContext
{
    public CineTecContext(DbContextOptions<CineTecContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
    public virtual DbSet<Pelicula> Peliculas { get; set; } = null!;
    public virtual DbSet<Sala> Salas { get; set; } = null!;
    public virtual DbSet<Funcion> Funciones { get; set; } = null!;
    public virtual DbSet<Butaca> Butacas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Correo).HasMaxLength(150).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Rol).HasMaxLength(20).HasDefaultValue("Cliente");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        });

        modelBuilder.Entity<Pelicula>(entity =>
        {
            entity.ToTable("peliculas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).HasMaxLength(150).IsRequired();
            entity.Property(e => e.DuracionMinutos).HasColumnName("duracion_minutos");
            entity.Property(e => e.Clasificacion).HasMaxLength(10);
            entity.Property(e => e.Genero).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Activa");
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.ToTable("salas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Funcion>(entity =>
        {
            entity.ToTable("funciones");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PeliculaId).HasColumnName("pelicula_id");
            entity.Property(e => e.SalaId).HasColumnName("sala_id");
            entity.Property(e => e.FechaHoraInicio).HasColumnName("fecha_hora_inicio");
            entity.Property(e => e.FechaHoraFin).HasColumnName("fecha_hora_fin");
            entity.Property(e => e.PrecioBase).HasColumnName("precio_base").HasPrecision(10, 2);
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Activa");

            entity.HasOne(d => d.Pelicula).WithMany(p => p.Funciones)
                .HasForeignKey(d => d.PeliculaId);

            entity.HasOne(d => d.Sala).WithMany(p => p.Funciones)
                .HasForeignKey(d => d.SalaId);
        });

        modelBuilder.Entity<Butaca>(entity =>
        {
            entity.ToTable("butacas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SalaId).HasColumnName("sala_id");
            entity.Property(e => e.Fila).HasMaxLength(1);
            entity.Property(e => e.Codigo).HasMaxLength(10);

            entity.HasOne(d => d.Sala).WithMany(p => p.Butacas)
                .HasForeignKey(d => d.SalaId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}