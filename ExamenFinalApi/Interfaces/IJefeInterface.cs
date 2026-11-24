using Contratos;

namespace ExamenFinalApi.Interfaces
{
    // El "contrato": QUÉ se puede hacer con los jefes (el CÓMO está en JefeService)
    public interface IJefeInterface
    {
        List<JefeRespuestaDto> ObtenerTodos();
        JefeRespuestaDto? ObtenerPorId(Guid id);

        // Devuelve el jefe creado, o un error (campo + mensaje) si no se pudo
        (JefeRespuestaDto? Jefe, ErrorDto? Error) Crear(JefeDto dto);
    }
}
