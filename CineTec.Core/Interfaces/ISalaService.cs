using CineTec.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface ISalaService
    {
        Task<IEnumerable<SalaDto>> GetAllRoomsAsync();
        Task<SalaDto> GetRoomByIdAsync(int id);
    }
}
