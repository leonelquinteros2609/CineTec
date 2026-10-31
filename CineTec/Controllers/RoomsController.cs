using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineTec.Api.Response;
using CineTec.Core.DTOs;
using CineTec.Core.Interfaces;

namespace CineTec.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class RoomsController : ControllerBase
    {
        private readonly ISalaService _salaService;

        public RoomsController(ISalaService salaService)
        {
            _salaService = salaService;
        }

        // CU-07: Consultar Salas para Programación (Administrador)
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var salas = await _salaService.GetAllRoomsAsync();
            return Ok(new ApiResponse<IEnumerable<SalaDto>>(salas));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var sala = await _salaService.GetRoomByIdAsync(id);
            return Ok(new ApiResponse<SalaDto>(sala));
        }
    }
}
