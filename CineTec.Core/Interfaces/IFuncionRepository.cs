using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface IFuncionRepository
    {
        Task<IEnumerable<FuncionDto>> GetActiveShowtimesAsync(string? fecha);
        Task<object?> GetSeatsByShowtimeIdAsync(int id);
        Task<Funcion?> GetByIdAsync(int id);
    }
}