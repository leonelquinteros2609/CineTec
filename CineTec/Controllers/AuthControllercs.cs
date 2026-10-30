using System.Linq;
using System.Threading.Tasks;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UsuarioDtoValidator _registerValidator;
        private readonly LoginDtoValidator _loginValidator;

        public AuthController(
            IAuthService authService,
            UsuarioDtoValidator registerValidator,
            LoginDtoValidator loginValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        // CU-01: Registrarse
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioDto? registerDto)
        {
            if (registerDto == null)
            {
                throw new BusinessException("Los datos de registro son obligatorios.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _registerValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación en los datos de registro",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var result = await _authService.RegisterAsync(registerDto);
            return Ok(new ApiResponse<AuthResponseDto>(result));
        }

        // CU-01: Iniciar Sesión
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto? loginDto)
        {
            if (loginDto == null)
            {
                throw new BusinessException("Las credenciales son obligatorias.", System.Net.HttpStatusCode.BadRequest);
            }

            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(
                    "Error de validación en las credenciales",
                    System.Net.HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
            }

            var result = await _authService.LoginAsync(loginDto);
            return Ok(new ApiResponse<AuthResponseDto>(result));
        }
    }
}
