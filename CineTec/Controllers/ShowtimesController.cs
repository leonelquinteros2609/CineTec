using Microsoft.AspNetCore.Mvc;
using CineTec.Core.Interfaces;
using System.Threading.Tasks;

namespace CineTec.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimesController : ControllerBase
    {
        private readonly IFuncionRepository _funcionRepository;

        public ShowtimesController(IFuncionRepository funcionRepository)
        {
            _funcionRepository = funcionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetShowtimes([FromQuery] string? fecha)
        {
            var funciones = await _funcionRepository.GetActiveShowtimesAsync(fecha);
            return Ok(funciones);
        }

        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetSeats(int id)
        {
            var seatsMap = await _funcionRepository.GetSeatsByShowtimeIdAsync(id);
            if (seatsMap == null)
            {
                return NotFound(new { message = "Función no disponible o inexistente" });
            }

            return Ok(seatsMap);
        }
    }
}