using System.ComponentModel.DataAnnotations;

namespace Contratos
{
    // Lo que se manda para REGISTRAR un proyecto
    public class ProyectoDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es obligatorio")]
        [RegularExpression("^(Pendiente|En progreso|Finalizado)$",
            ErrorMessage = "El estado debe ser Pendiente, En progreso o Finalizado")]
        public string Estado { get; set; } = "Pendiente";

        [Required(ErrorMessage = "Selecciona un jefe")]
        public Guid IdJefe { get; set; }
    }

    // Lo que la API DEVUELVE de un proyecto (con el nombre de su jefe)
    public class ProyectoRespuestaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public Guid IdJefe { get; set; }
        public string NombreJefe { get; set; } = string.Empty;
        public string CargoJefe { get; set; } = string.Empty;
        public string? FotoJefe { get; set; }
    }

    // Respuesta de error de la API: en qué campo falló y por qué
    public class ErrorDto
    {
        public string Campo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }

    public static class Estados
    {
        public static readonly string[] Todos = { "Pendiente", "En progreso", "Finalizado" };
    }
}
