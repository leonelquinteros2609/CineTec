using System.Reflection;
using Microsoft.EntityFrameworkCore;
using CineTec.Core.Entities;

namespace CineTec.Infrastructure.Data
{
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
        public virtual DbSet<Compra> Compras { get; set; } = null!;
        public virtual DbSet<Entrada> Entradas { get; set; } = null!;
        public virtual DbSet<Pago> Pagos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
