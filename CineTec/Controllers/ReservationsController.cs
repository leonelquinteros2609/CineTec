using CineTec.Core.DTOs;
using CineTec.Core.Entities;
using CineTec.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineTec.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IFuncionRepository _funcionRepository;
    private readonly IUsuarioRepository _userRepository;

    public ReservationsController(
        IReservaRepository reservaRepository,
        IFuncionRepository funcionRepository,
        IUsuarioRepository userRepository)
    {
        _reservaRepository = reservaRepository;
        _funcionRepository = funcionRepository;
        _userRepository = userRepository;
    }

    // CU-06: Reservar Butacas
    [HttpPost]
    public async Task<IActionResult> CrearReserva([FromBody] CrearReservaDto dto)
    {
        var user = await _userRepository.GetUserByIdAsync(dto.UsuarioId);
        if (user == null) return BadRequest(new { mensaje = "El usuario no existe." });

        var funcion = await _funcionRepository.GetByIdAsync(dto.FuncionId);
        if (funcion == null || funcion.Estado != "Activa")
            return NotFound(new { mensaje = "Función no disponible o inexistente." });

        if (funcion.FechaHoraInicio <= DateTime.Now)
            return BadRequest(new { mensaje = "La función ya comenzó." });

        var ocupadas = await _reservaRepository.GetButacasOcupadasByFuncionIdAsync(dto.FuncionId);
        if (dto.ButacaIds.Any(bId => ocupadas.Contains(bId)))
            return Conflict(new { mensaje = "Una o más butacas ya están reservadas." });

        var reserva = new Reserva
        {
            UsuarioId = dto.UsuarioId,
            FuncionId = dto.FuncionId,
            MontoTotal = dto.ButacaIds.Count * funcion.PrecioBase,
            Estado = "Confirmada",
            ReservaButacas = dto.ButacaIds.Select(id => new ReservaButaca { ButacaId = id }).ToList()
        };

        await _reservaRepository.InsertAsync(reserva);
        return CreatedAtAction(nameof(ObtenerMisReservas), new { usuarioId = dto.UsuarioId }, reserva);
    }

    // CU-07: Consultar Mis Reservas
    [HttpGet("me")]
    public async Task<IActionResult> ObtenerMisReservas([FromQuery] int usuarioId)
    {
        var reservas = await _reservaRepository.GetByUsuarioIdAsync(usuarioId);
        return Ok(reservas);
    }

    // CU-08: Cancelar Reserva
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelarReserva(int id, [FromQuery] int usuarioId)
    {
        var reserva = await _reservaRepository.GetByIdAsync(id);
        if (reserva == null) return NotFound(new { mensaje = "Reserva no encontrada." });

        if (reserva.UsuarioId != usuarioId)
            return StatusCode(403, new { mensaje = "No tiene permiso sobre esta reserva." });

        if (reserva.Funcion.FechaHoraInicio <= DateTime.Now)
            return BadRequest(new { mensaje = "La función ya comenzó." });

        reserva.Estado = "Cancelada";
        await _reservaRepository.UpdateAsync(reserva);
        return NoContent();
    }
}
