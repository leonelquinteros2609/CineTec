using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface IUsuarioService
    {
        Task<AuthResponseDto> RegisterAsync(UsuarioDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<Usuario?> GetUserByIdAsync(int id);
    }
}
