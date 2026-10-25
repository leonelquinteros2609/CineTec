using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using CineTec.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CineTec.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD MySql
            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
            builder.Services.AddDbContext<CineTecContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            #endregion

            #region Inyección de Dependencias
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            builder.Services.AddScoped<IFuncionRepository, FuncionRepository>();
            builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
            #endregion

            // Add services to the container.
            builder.Services.AddControllers();

            // OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            #region Crear Tablas en MySQL Automáticamente
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CineTecContext>();

                // 1. Crear la base de datos cinetec_db si no existe
                dbContext.Database.EnsureCreated();

                // 2. Crear tabla reservas
                dbContext.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS reservas (
            id INT NOT NULL AUTO_INCREMENT,
            usuario_id INT NOT NULL,
            funcion_id INT NOT NULL,
            fecha_reserva DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            monto_total DECIMAL(10,2) NOT NULL,
            estado VARCHAR(20) NOT NULL DEFAULT 'Confirmada',
            PRIMARY KEY (id),
            CONSTRAINT fk_reservas_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios (id),
            CONSTRAINT fk_reservas_funcion FOREIGN KEY (funcion_id) REFERENCES funciones (id)
        ) ENGINE=InnoDB;
    ");

                // 3. Crear tabla reserva_butacas
                dbContext.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS reserva_butacas (
            id INT NOT NULL AUTO_INCREMENT,
            reserva_id INT NOT NULL,
            butaca_id INT NOT NULL,
            PRIMARY KEY (id),
            CONSTRAINT fk_reserva_butacas_reserva FOREIGN KEY (reserva_id) REFERENCES reservas (id) ON DELETE CASCADE,
            CONSTRAINT fk_reserva_butacas_butaca FOREIGN KEY (butaca_id) REFERENCES butacas (id),
            UNIQUE KEY uk_reserva_butaca (reserva_id, butaca_id)
        ) ENGINE=InnoDB;
    ");
            }
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}