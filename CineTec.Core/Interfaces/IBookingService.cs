using CineTec.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface IBookingService
    {
        Task<ReservaResponseDto> CreateBookingAsync(int usuarioId, CrearReservaDto dto);
        Task<CompraDetalleDto> PayBookingAsync(int usuarioId, int compraId, PagarReservaDto dto);
        Task<IEnumerable<CompraDetalleDto>> GetUserBookingsAsync(int usuarioId);
        Task<CompraDetalleDto> GetBookingByIdAsync(int usuarioId, int compraId);
        Task<CompraDetalleDto> CancelBookingAsync(int usuarioId, int compraId);
    }
}
