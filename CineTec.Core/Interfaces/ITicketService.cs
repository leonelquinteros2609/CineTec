using CineTec.Core.DTOs;
using System.Threading.Tasks;

namespace CineTec.Core.Interfaces
{
    public interface ITicketService
    {
        Task<TicketValidadoDto> ValidateTicketAsync(ValidarTicketDto dto);
    }
}
