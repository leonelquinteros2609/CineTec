using Microsoft.AspNetCore.Mvc;
using CineTec.Core.Interfaces;
using System.Threading.Tasks;

namespace CineTec.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IPeliculaRepository _peliculaRepository;

        public MoviesController(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var peliculas = await _peliculaRepository.GetAllActiveMoviesAsync();
            return Ok(peliculas);
        }
    }
}