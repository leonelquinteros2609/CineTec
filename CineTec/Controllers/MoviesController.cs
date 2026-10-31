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
    public class MoviesController : ControllerBase
    {
        private readonly IPeliculaService _peliculaService;
        private readonly CrearPeliculaDtoValidator _crearValidator;
        private readonly ActualizarPeliculaDtoValidator _actualizarValidator;

        public MoviesController(
            IPeliculaService peliculaService,
            CrearPeliculaDtoValidator crearValidator,
            ActualizarPeliculaDtoValidator actualizarValidator)
        {
            _peliculaService = peliculaService;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        // CU-02: Consultar Cartelera (Público / Clientes)
        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery] bool? todas = false)
        {
            var peliculas = (todas == true)
                ? await _peliculaService.GetAllMoviesAsync()
                : await _peliculaService.GetAllActiveMoviesAsync();

            return Ok(new ApiResponse<IEnumerable<PeliculaDto>>(peliculas));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var pelicula = await _peliculaService.GetMovieByIdAsync(id);
            return Ok(new ApiResponse<PeliculaDto>(pelicula));
        }

        // CU-07: Gestionar Películas (Administrador)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateMovie([FromBody] CrearPeliculaDto? dto)
        {
            if (dto == null)
            {
                throw new BusinessException("Los datos de la película son obligatorios.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _crearValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación al crear la película",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var nuevaPelicula = await _peliculaService.CreateMovieAsync(dto);
            return CreatedAtAction(nameof(GetMovieById), new { id = nuevaPelicula.Id }, new ApiResponse<PeliculaDto>(nuevaPelicula));
        }

        // CU-07: Editar Película (Administrador)
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] ActualizarPeliculaDto? dto)
        {
            if (dto == null)
            {
                throw new BusinessException("Los datos de la película son obligatorios.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _actualizarValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación al actualizar la película",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var peliculaActualizada = await _peliculaService.UpdateMovieAsync(id, dto);
            return Ok(new ApiResponse<PeliculaDto>(peliculaActualizada));
        }

        // CU-07: Eliminar o Desactivar Película (Administrador, RN-08)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            await _peliculaService.DeleteMovieAsync(id);
            return NoContent();
        }
    }
}
