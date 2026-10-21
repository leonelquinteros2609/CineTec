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
            #endregion

            // Add services to the container.
            builder.Services.AddControllers();

            // OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

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