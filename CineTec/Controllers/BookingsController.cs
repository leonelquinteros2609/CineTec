using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly CrearReservaDtoValidator _crearValidator;
        private readonly PagarReservaDtoValidator _pagarValidator;

        public BookingsController(
            IBookingService bookingService,
            CrearReservaDtoValidator crearValidator,
            PagarReservaDtoValidator pagarValidator)
        {
            _bookingService = bookingService;
            _crearValidator = crearValidator;
            _pagarValidator = pagarValidator;
        }

        // CU-03: Reservar Butacas (Cliente)
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CrearReservaDto? dto)
        {
            if (dto == null)
            {
                throw new BusinessException("Los datos de la reserva son obligatorios.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _crearValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación al reservar butacas",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var userId = GetCurrentUserId();
            var reserva = await _bookingService.CreateBookingAsync(userId, dto);

            return StatusCode(201, new ApiResponse<ReservaResponseDto>(reserva));
        }

        // CU-04: Confirmar Compra de Entradas (Pagar) (Cliente)
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayBooking(int id, [FromBody] PagarReservaDto? dto)
        {
            if (dto == null)
            {
                dto = new PagarReservaDto();
            }

            var validationResult = await _pagarValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación en los datos de pago",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var userId = GetCurrentUserId();
            var compraPagada = await _bookingService.PayBookingAsync(userId, id, dto);

            return Ok(new ApiResponse<CompraDetalleDto>(compraPagada));
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id") ?? User.FindFirst("sub");
            if (claim != null && int.TryParse(claim.Value, out var id))
            {
                return id;
            }

            if (Request.Headers.TryGetValue("X-User-Id", out var headerVal) && int.TryParse(headerVal, out var headerId))
            {
                return headerId;
            }

            throw new BusinessException("Debe iniciar sesión para realizar esta operación.", System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
