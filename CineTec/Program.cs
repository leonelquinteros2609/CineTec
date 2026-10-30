using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            #region Inyección de Dependencias - Servicios de Negocio
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IPeliculaService, PeliculaService>();
            builder.Services.AddScoped<IFuncionService, FuncionService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ISalaService, SalaService>();
            #endregion

            #region AutoMapper
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CineTecProfile).Assembly));
            #endregion

            #region Validadores FluentValidation
            builder.Services.AddScoped<UsuarioDtoValidator>();
            builder.Services.AddScoped<LoginDtoValidator>();
            builder.Services.AddScoped<CrearPeliculaDtoValidator>();
            builder.Services.AddScoped<ActualizarPeliculaDtoValidator>();
            builder.Services.AddScoped<CrearFuncionDtoValidator>();
            builder.Services.AddScoped<ActualizarFuncionDtoValidator>();
            builder.Services.AddScoped<CrearReservaDtoValidator>();
            builder.Services.AddScoped<PagarReservaDtoValidator>();
            builder.Services.AddScoped<ValidarTicketDtoValidator>();
            #endregion

            #region Configuración de Autenticación y JWT
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "CineTec_Super_Secret_Key_For_JWT_Authentication_2026_UCB!";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CineTecApi";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CineTecClient";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();
            #endregion

            // Configurar Controllers y suprimir el filtro automático de ModelState para usar ExceptionHandlingMiddleware
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Middleware global de manejo de excepciones estructuradas (siguiendo SocialMediaStudent)
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
