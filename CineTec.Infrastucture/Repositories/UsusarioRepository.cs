using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Helpers;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace CineTec.Infrastructure.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(CineTecContext context) : base(context)
        {
        }

        public async Task<(bool Success, string Message, object? Data)> RegisterAsync(UsuarioDto dto)
        {
            var email = dto.GetEmail();
            var existe = await _entities.AnyAsync(u => u.Email == email);
            if (existe)
            {
                return (false, "El correo electrónico ya se encuentra registrado", null);
            }

            var passwordHash = PasswordHelper.HashPassword(dto.Password);
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = email,
                PasswordHash = passwordHash,
                Rol = string.IsNullOrWhiteSpace(dto.Rol) ? "Cliente" : dto.Rol,
                FechaCreacion = DateTime.Now
            };

            await Add(usuario);

            return (true, "Usuario registrado exitosamente", new { usuario.Id, usuario.Nombre, Email = usuario.Email, usuario.Rol });
        }

        public async Task<(bool Success, string Message, object? Data)> LoginAsync(LoginDto dto)
        {
            var email = dto.GetEmail();
            var usuario = await _entities.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
            {
                return (false, "Credenciales inválidas", null);
            }

            bool passwordValida = PasswordHelper.VerifyPassword(dto.Password, usuario.PasswordHash);

            if (!passwordValida)
            {
                return (false, "Credenciales inválidas", null);
            }

            return (true, "Inicio de sesión exitoso", new { usuario.Id, usuario.Nombre, Email = usuario.Email, usuario.Rol });
        }
    }
}
