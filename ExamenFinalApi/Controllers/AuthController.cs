using Contratos;
using Microsoft.AspNetCore.Mvc;
using ExamenFinalApi.Interfaces;

namespace ExamenFinalApi.Controllers
{
    // SIN [Authorize]: si no, nadie podría iniciar sesión para sacar su token
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authService;

        public AuthController(IAuthInterface authService)
        {
            _authService = authService;
        }

        // POST api/Auth/registrar  -> crea un usuario con rol "Usuario"
        [HttpPost("registrar")]
        public IActionResult Registrar(LoginDto dto)
        {
            var error = _authService.Registrar(dto);
            if (error != null)
                return Conflict(error);

            return Ok("Usuario registrado. Ya puedes iniciar sesión.");
        }

        // POST api/Auth/login  -> devuelve el token
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var respuesta = _authService.Login(dto);
            if (respuesta == null)
                return Unauthorized("Usuario o contraseña incorrectos.");

            return Ok(respuesta);
        }
    }
}
