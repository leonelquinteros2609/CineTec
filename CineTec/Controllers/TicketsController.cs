using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineTec.Api.Response;
using CineTec.Core.DTOs;
using CineTec.Core.Exceptions;
using CineTec.Core.Interfaces;
using CineTec.Services.Validators;

namespace CineTec.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ValidarTicketDtoValidator _validator;

        public TicketsController(
            ITicketService ticketService,
            ValidarTicketDtoValidator validator)
        {
            _ticketService = ticketService;
            _validator = validator;
        }

        // CU-08: Validar Entrada en Puerta (Taquillero o Administrador)
        [HttpPost("validate")]
        [Authorize(Roles = "Taquillero,Administrador")]
        public async Task<IActionResult> ValidateTicket([FromBody] ValidarTicketDto? dto)
        {
            if (dto == null)
            {
                throw new BusinessException("El código de la entrada es obligatorio.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación al validar la entrada",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var resultado = await _ticketService.ValidateTicketAsync(dto);
            return Ok(new ApiResponse<TicketValidadoDto>(resultado));
        }
    }
}
