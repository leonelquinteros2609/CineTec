using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Authorize]
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

            return CreatedAtAction(nameof(GetBookingById), new { id = reserva.CompraId }, new ApiResponse<ReservaResponseDto>(reserva));
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

        // CU-05: Consultar Mis Compras (Cliente)
        [HttpGet]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetCurrentUserId();
            var compras = await _bookingService.GetUserBookingsAsync(userId);

            return Ok(new ApiResponse<IEnumerable<CompraDetalleDto>>(compras));
        }

        // CU-05: Detalle de Compra (Cliente)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var userId = GetCurrentUserId();
            var compra = await _bookingService.GetBookingByIdAsync(userId, id);

            return Ok(new ApiResponse<CompraDetalleDto>(compra));
        }

        // CU-06: Cancelar Compra (Cliente)
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = GetCurrentUserId();
            var compraCancelada = await _bookingService.CancelBookingAsync(userId, id);

            return Ok(new ApiResponse<CompraDetalleDto>(compraCancelada));
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
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
