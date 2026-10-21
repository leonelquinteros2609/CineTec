using Microsoft.AspNetCore.Mvc;
using CineTec.Core.DTOs;
using CineTec.Core.Interfaces;
using System.Threading.Tasks;

namespace CineTec.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioDto registerDto)
        {
            var result = await _usuarioRepository.RegisterAsync(registerDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _usuarioRepository.LoginAsync(loginDto);
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(result);
        }
    }
}