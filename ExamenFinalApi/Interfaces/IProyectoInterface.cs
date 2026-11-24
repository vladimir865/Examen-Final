using Contratos;

namespace ExamenFinalApi.Interfaces
{
    // El "contrato": QUÉ se puede hacer con los proyectos (el CÓMO está en ProyectoService)
    public interface IProyectoInterface
    {
        List<ProyectoRespuestaDto> ObtenerTodos();
        ProyectoRespuestaDto? ObtenerPorId(Guid id);

        // Devuelve el proyecto creado, o un error (campo + mensaje) si no se pudo
        (ProyectoRespuestaDto? Proyecto, ErrorDto? Error) Crear(ProyectoDto dto);
    }
}
