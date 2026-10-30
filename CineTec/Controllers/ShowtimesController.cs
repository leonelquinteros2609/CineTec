using System.Collections.Generic;
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
    public class ShowtimesController : ControllerBase
    {
        private readonly IFuncionService _funcionService;
        private readonly CrearFuncionDtoValidator _crearValidator;
        private readonly ActualizarFuncionDtoValidator _actualizarValidator;

        public ShowtimesController(
            IFuncionService funcionService,
            CrearFuncionDtoValidator crearValidator,
            ActualizarFuncionDtoValidator actualizarValidator)
        {
            _funcionService = funcionService;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
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

        // CU-07: Gestionar Funciones - Crear (Administrador, RN-07)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateShowtime([FromBody] CrearFuncionDto? dto)
        {
            if (dto == null)
            {
                throw new BusinessException("Los datos de la función son obligatorios.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _crearValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación al crear la función",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var nuevaFuncion = await _funcionService.CreateShowtimeAsync(dto);
            return CreatedAtAction(nameof(GetShowtimeById), new { id = nuevaFuncion.FuncionId }, new ApiResponse<FuncionDto>(nuevaFuncion));
        }

        // CU-07: Gestionar Funciones - Editar (Administrador, RN-07)
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateShowtime(int id, [FromBody] ActualizarFuncionDto? dto)
        {
            if (dto == null)
            {
                throw new BusinessException("Los datos de la función son obligatorios.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _actualizarValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación al actualizar la función",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var funcionActualizada = await _funcionService.UpdateShowtimeAsync(id, dto);
            return Ok(new ApiResponse<FuncionDto>(funcionActualizada));
        }

        // CU-07: Gestionar Funciones - Eliminar / Cancelar (Administrador, RN-08)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteShowtime(int id)
        {
            await _funcionService.DeleteShowtimeAsync(id);
            return NoContent();
        }
    }
}
