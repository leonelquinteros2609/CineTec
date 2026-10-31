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
    public class MoviesController : ControllerBase
    {
        private readonly IPeliculaService _peliculaService;

        public MoviesController(IPeliculaService peliculaService)
        {
            _peliculaService = peliculaService;
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
    }
}
