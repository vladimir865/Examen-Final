using Contratos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamenFinalApi.Interfaces;

namespace ExamenFinalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]   // sin token -> 401
    public class ProyectoController : ControllerBase
    {
        private readonly IProyectoInterface _proyectoService;

        public ProyectoController(IProyectoInterface proyectoService)
        {
            _proyectoService = proyectoService;
        }

        // GET api/Proyecto
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_proyectoService.ObtenerTodos());
        }

        // 3) GET api/Proyecto/{id}  -> buscar proyecto por ID
        [HttpGet("{id}")]
        public IActionResult GetPorId(Guid id)
        {
            var proyecto = _proyectoService.ObtenerPorId(id);
            if (proyecto == null)
                return NotFound(new ErrorDto { Campo = "Id", Mensaje = "No existe un proyecto con ese ID." });

            return Ok(proyecto);
        }

        // 2) POST api/Proyecto  -> registrar proyecto
        [HttpPost]
        [Authorize(Roles = "Administrador")]   // el Usuario solo puede ver
        public IActionResult Post(ProyectoDto dto)
        {
            var (proyecto, error) = _proyectoService.Crear(dto);
            if (error != null)
            {
                // 400 si el jefe no existe; 409 si el nombre se repite o el jefe ya tiene proyecto
                return error.Mensaje.StartsWith("Selecciona") ? BadRequest(error) : Conflict(error);
            }

            return CreatedAtAction(nameof(GetPorId), new { id = proyecto!.Id }, proyecto);
        }
    }
}
