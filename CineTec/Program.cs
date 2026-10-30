using Microsoft.EntityFrameworkCore;
using CineTec.Api.Filters;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using CineTec.Infrastructure.Mappings;
using CineTec.Infrastructure.Repositories;
using CineTec.Services;
using CineTec.Services.Validators;

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

            #region Inyección de Dependencias - Repositorios
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            builder.Services.AddScoped<IFuncionRepository, FuncionRepository>();
            #endregion

            #region Inyección de Dependencias - Servicios de Negocio (CU-01 a CU-04)
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IPeliculaService, PeliculaService>();
            builder.Services.AddScoped<IFuncionService, FuncionService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            #endregion

            #region AutoMapper
            builder.Services.AddAutoMapper(typeof(CineTecProfile).Assembly);
            #endregion

            #region Validadores FluentValidation (CU-01 a CU-04)
            builder.Services.AddScoped<UsuarioDtoValidator>();
            builder.Services.AddScoped<LoginDtoValidator>();
            builder.Services.AddScoped<CrearReservaDtoValidator>();
            builder.Services.AddScoped<PagarReservaDtoValidator>();
            #endregion

            builder.Services.AddAuthorization();

            // Configurar Controllers y suprimir el filtro automático de ModelState para usar ExceptionHandlingMiddleware (patrón SocialMediaStudent)
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Middleware global de manejo de excepciones estructuradas (siguiendo SocialMediaStudent)
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Middleware de autenticación JWT
            app.UseMiddleware<JwtAuthenticationMiddleware>();

            // Configurar Swagger/OpenAPI
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
