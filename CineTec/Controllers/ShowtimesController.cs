using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CineTec.Api.Response;
using CineTec.Core.DTOs;
using CineTec.Core.Interfaces;

namespace CineTec.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimesController : ControllerBase
    {
        private readonly IFuncionService _funcionService;

        public ShowtimesController(IFuncionService funcionService)
        {
            _funcionService = funcionService;
        }

        // CU-02: Consultar Cartelera y Funciones (RN-04)
        [HttpGet]
        public async Task<IActionResult> GetShowtimes([FromQuery] string? fecha, [FromQuery] int? peliculaId)
        {
            var funciones = await _funcionService.GetActiveShowtimesAsync(fecha, peliculaId);
            return Ok(new ApiResponse<IEnumerable<FuncionDto>>(funciones));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowtimeById(int id)
        {
            var funcion = await _funcionService.GetShowtimeByIdAsync(id);
            return Ok(new ApiResponse<FuncionDto>(funcion));
        }

        // CU-02: Consultar Mapa de Butacas y Disponibilidad (RN-01, RN-02)
        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetSeats(int id)
        {
            var seatsMap = await _funcionService.GetSeatsMapByShowtimeIdAsync(id);
            return Ok(new ApiResponse<MapaButacasDto>(seatsMap));
        }
    }
}
