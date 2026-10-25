using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<(bool Success, string Message, object? Data)> RegisterAsync(UsuarioDto dto);
        Task<(bool Success, string Message, object? Data)> LoginAsync(LoginDto dto);
        Task<Usuario?> GetUserByIdAsync(int id); // <--- Método necesario para las reservas
    }
}