using Microsoft.EntityFrameworkCore;
using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using CineTec.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineTec.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly CineTecContext _cineTecContext;

        public UsuarioRepository(CineTecContext cineTecContext)
        {
            _cineTecContext = cineTecContext;
        }

        public async Task<(bool Success, string Message, object? Data)> RegisterAsync(UsuarioDto dto)
        {
            var existe = await _cineTecContext.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
            if (existe)
            {
                return (false, "El correo electrónico ya se encuentra registrado", null);
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = dto.Password, // Comparación y guardado directo en texto plano
                Rol = string.IsNullOrEmpty(dto.Rol) ? "Cliente" : dto.Rol,
                FechaCreacion = DateTime.Now
            };

            _cineTecContext.Usuarios.Add(usuario);
            await _cineTecContext.SaveChangesAsync();

            var resultado = (true, "Usuario registrado exitosamente", (object)new { usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol });
            return resultado;
        }

        public async Task<(bool Success, string Message, object? Data)> LoginAsync(LoginDto dto)
        {
            var usuario = await _cineTecContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null || usuario.PasswordHash != dto.Password)
            {
                return (false, "Credenciales inválidas", null);
            }

            var resultado = (true, "Inicio de sesión exitoso", (object)new { usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol });
            return resultado;
        }
    }
}