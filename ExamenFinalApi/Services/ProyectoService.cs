using Contratos;
using Microsoft.EntityFrameworkCore;
using ExamenFinalApi.Data;
using ExamenFinalApi.Interfaces;
using ExamenFinalApi.Models;

namespace ExamenFinalApi.Services
{
    public class ProyectoService : IProyectoInterface
    {
        private readonly BaseDatos _context;

        public ProyectoService(BaseDatos context)
        {
            _context = context;
        }

        public List<ProyectoRespuestaDto> ObtenerTodos()
        {
            return _context.Proyectos
                .Include(p => p.Jefe)
                .OrderBy(p => p.Nombre)
                .Select(p => new ProyectoRespuestaDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Estado = p.Estado,
                    IdJefe = p.IdJefe,
                    NombreJefe = p.Jefe!.Nombre,
                    CargoJefe = p.Jefe.Cargo,
                    FotoJefe = p.Jefe.Foto
                })
                .ToList();
        }

        // Buscar proyecto por ID
        public ProyectoRespuestaDto? ObtenerPorId(Guid id)
        {
            return _context.Proyectos
                .Include(p => p.Jefe)
                .Where(p => p.Id == id)
                .Select(p => new ProyectoRespuestaDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Estado = p.Estado,
                    IdJefe = p.IdJefe,
                    NombreJefe = p.Jefe!.Nombre,
                    CargoJefe = p.Jefe.Cargo,
                    FotoJefe = p.Jefe.Foto
                })
                .FirstOrDefault();
        }

        public (ProyectoRespuestaDto? Proyecto, ErrorDto? Error) Crear(ProyectoDto dto)
        {
            var nombre = dto.Nombre.Trim();

            // REGLA: no puede haber 2 proyectos con el mismo nombre
            if (_context.Proyectos.Any(p => p.Nombre.ToLower() == nombre.ToLower()))
            {
                return (null, new ErrorDto { Campo = "Nombre", Mensaje = "Ya existe un proyecto con ese nombre." });
            }

            // El jefe tiene que existir
            if (dto.IdJefe == Guid.Empty || !_context.Jefes.Any(j => j.Id == dto.IdJefe))
            {
                return (null, new ErrorDto { Campo = "IdJefe", Mensaje = "Selecciona un jefe que exista." });
            }

            // REGLA: un jefe solo puede estar a cargo de 1 proyecto
            var proyectoDelJefe = _context.Proyectos.FirstOrDefault(p => p.IdJefe == dto.IdJefe);
            if (proyectoDelJefe != null)
            {
                return (null, new ErrorDto
                {
                    Campo = "IdJefe",
                    Mensaje = $"Ese jefe ya está a cargo del proyecto \"{proyectoDelJefe.Nombre}\"."
                });
            }

            var proyecto = new Proyecto
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Descripcion = dto.Descripcion.Trim(),
                Estado = dto.Estado,
                IdJefe = dto.IdJefe
            };

            _context.Proyectos.Add(proyecto);
            _context.SaveChanges();

            return (ObtenerPorId(proyecto.Id), null);
        }
    }
}
