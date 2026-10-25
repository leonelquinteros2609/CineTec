using CineTec.Core.Entities;

namespace CineTec.Core.Interfaces;

public interface IReservaRepository
{
    Task<Reserva?> GetByIdAsync(int id);
    Task<IEnumerable<Reserva>> GetByUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<int>> GetButacasOcupadasByFuncionIdAsync(int funcionId);
    Task InsertAsync(Reserva reserva);
    Task UpdateAsync(Reserva reserva);
}