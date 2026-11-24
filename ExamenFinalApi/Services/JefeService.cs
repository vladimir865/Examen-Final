using Contratos;
using Microsoft.EntityFrameworkCore;
using ExamenFinalApi.Data;
using ExamenFinalApi.Interfaces;
using ExamenFinalApi.Models;

namespace ExamenFinalApi.Services
{
    public class JefeService : IJefeInterface
    {
        private readonly BaseDatos _context;

        public JefeService(BaseDatos context)
        {
            _context = context;
        }

        public List<JefeRespuestaDto> ObtenerTodos()
        {
            return _context.Jefes
                .Include(j => j.Proyecto)          // para saber si ya tiene proyecto
                .OrderBy(j => j.Nombre)
                .Select(j => new JefeRespuestaDto
                {
                    Id = j.Id,
                    Nombre = j.Nombre,
                    Cargo = j.Cargo,
                    Correo = j.Correo,
                    Foto = j.Foto,
                    ProyectoNombre = j.Proyecto != null ? j.Proyecto.Nombre : null
                })
                .ToList();
        }

        public JefeRespuestaDto? ObtenerPorId(Guid id)
        {
            return _context.Jefes
                .Include(j => j.Proyecto)
                .Where(j => j.Id == id)
                .Select(j => new JefeRespuestaDto
                {
                    Id = j.Id,
                    Nombre = j.Nombre,
                    Cargo = j.Cargo,
                    Correo = j.Correo,
                    Foto = j.Foto,
                    ProyectoNombre = j.Proyecto != null ? j.Proyecto.Nombre : null
                })
                .FirstOrDefault();
        }

        public (JefeRespuestaDto? Jefe, ErrorDto? Error) Crear(JefeDto dto)
        {
            var nombre = dto.Nombre.Trim();

            // REGLA: no puede haber 2 jefes con el mismo nombre (sin importar mayúsculas)
            if (_context.Jefes.Any(j => j.Nombre.ToLower() == nombre.ToLower()))
            {
                return (null, new ErrorDto { Campo = "Nombre", Mensaje = "Ya existe un jefe con ese nombre." });
            }

            var jefe = new Jefe
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Cargo = dto.Cargo.Trim(),
                Correo = dto.Correo.Trim().ToLower(),
                Foto = string.IsNullOrWhiteSpace(dto.Foto) ? null : dto.Foto.Trim()
            };

            _context.Jefes.Add(jefe);
            _context.SaveChanges();

            return (ObtenerPorId(jefe.Id), null);
        }
    }
}
