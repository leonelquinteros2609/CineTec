using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Exceptions;
using CineTec.Core.Helpers;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;

namespace CineTec.Services
{
    public class AuthService : IAuthService, IUsuarioService
    {
        private readonly CineTecContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(CineTecContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(UsuarioDto dto)
        {
            var email = dto.GetEmail().Trim().ToLowerInvariant();

            // RN-06: El correo electrónico de cada usuario debe ser único en el sistema
            var existe = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == email);
            if (existe)
            {
                throw new BusinessException("El correo electrónico ya se encuentra registrado", HttpStatusCode.BadRequest);
            }

            var passwordHash = PasswordHelper.HashPassword(dto.Password);
            var rol = string.IsNullOrWhiteSpace(dto.Rol) ? "Cliente" : dto.Rol.Trim();

            // Validar que el rol sea uno de los válidos según especificación
            if (rol != "Cliente" && rol != "Taquillero" && rol != "Administrador")
            {
                rol = "Cliente";
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                Rol = rol,
                FechaCreacion = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var (token, expiration) = GenerateJwtToken(usuario);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = expiration,
                Usuario = new UsuarioInfoResponseDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var email = dto.GetEmail().Trim().ToLowerInvariant();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (usuario == null)
            {
                throw new BusinessException("Credenciales inválidas", HttpStatusCode.Unauthorized);
            }

            bool passwordValida = PasswordHelper.VerifyPassword(dto.Password, usuario.PasswordHash);

            if (!passwordValida)
            {
                throw new BusinessException("Credenciales inválidas", HttpStatusCode.Unauthorized);
            }

            var (token, expiration) = GenerateJwtToken(usuario);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = expiration,
                Usuario = new UsuarioInfoResponseDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol
                }
            };
        }

        public async Task<Usuario?> GetUserByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        private (string Token, DateTime Expiration) GenerateJwtToken(Usuario usuario)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "CineTec_Super_Secret_Key_For_JWT_Authentication_2026_UCB!";
            var issuer = _configuration["Jwt:Issuer"] ?? "CineTecApi";
            var audience = _configuration["Jwt:Audience"] ?? "CineTecClient";
            var minutes = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var m) ? m : 120;

            var expiration = DateTime.UtcNow.AddMinutes(minutes);

            var token = JwtHelper.GenerateToken(
                jwtKey,
                issuer,
                audience,
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol,
                expiration
            );

            return (token, expiration);
        }
    }
}
