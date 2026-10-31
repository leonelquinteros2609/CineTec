using CineTec.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface IFuncionService
    {
        Task<IEnumerable<FuncionDto>> GetActiveShowtimesAsync(string? fecha, int? peliculaId = null);
        Task<IEnumerable<FuncionDto>> GetAllShowtimesAsync();
        Task<FuncionDto> GetShowtimeByIdAsync(int id);
        Task<MapaButacasDto> GetSeatsMapByShowtimeIdAsync(int showtimeId);
        Task<FuncionDto> CreateShowtimeAsync(CrearFuncionDto dto);
        Task<FuncionDto> UpdateShowtimeAsync(int id, ActualizarFuncionDto dto);
        Task DeleteShowtimeAsync(int id);
    }
}
