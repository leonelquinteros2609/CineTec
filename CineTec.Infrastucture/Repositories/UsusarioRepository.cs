using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
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

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
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

            bool passwordValida = false;
            try
            {
                passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
            }
            catch
            {
                // Soporte para contraseñas en texto plano heredadas
                passwordValida = (usuario.PasswordHash == dto.Password);
            }

            if (!passwordValida && usuario.PasswordHash == dto.Password)
            {
                passwordValida = true;
            }

            if (!passwordValida)
            {
                return (false, "Credenciales inválidas", null);
            }

            return (true, "Inicio de sesión exitoso", new { usuario.Id, usuario.Nombre, Email = usuario.Email, usuario.Rol });
        }
    }
}
