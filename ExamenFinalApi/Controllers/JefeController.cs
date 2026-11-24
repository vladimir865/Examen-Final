using Contratos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamenFinalApi.Interfaces;

namespace ExamenFinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // sin token -> 401
    public class JefeController : ControllerBase
    {
        private readonly IJefeInterface _jefeService;

        public JefeController(IJefeInterface jefeService)
        {
            _jefeService = jefeService;
        }

        // GET api/Jefe
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_jefeService.ObtenerTodos());
        }

        // GET api/Jefe/{id}
        [HttpGet("{id}")]
        public IActionResult GetPorId(Guid id)
        {
            var jefe = _jefeService.ObtenerPorId(id);
            if (jefe == null)
                return NotFound(new ErrorDto { Campo = "Id", Mensaje = "El jefe no existe." });

            return Ok(jefe);
        }

        // 1) POST api/Jefe  -> registrar jefe (recibe un JSON)
        [HttpPost]
        [Authorize(Roles = "Administrador")]   // el Usuario solo puede ver
        public IActionResult Post(JefeDto dto)
        {
            var (jefe, error) = _jefeService.Crear(dto);
            if (error != null)
                return Conflict(error);   // 409: nombre repetido

            return CreatedAtAction(nameof(GetPorId), new { id = jefe!.Id }, jefe);
        }
    }
}
