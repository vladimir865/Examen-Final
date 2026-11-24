using System.ComponentModel.DataAnnotations;

namespace ExamenFinalApi.Models
{
    // Tabla "jefes"
    public class Jefe
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;     // único: no se repite

        [Required]
        public string Cargo { get; set; } = string.Empty;

        [Required]
        public string Correo { get; set; } = string.Empty;

        // Ruta de la foto guardada en el servidor (ej: imagenes/abc.jpg). Opcional.
        public string? Foto { get; set; }

        // Relación 1 a 1: un jefe está a cargo de un solo proyecto (o de ninguno)
        public Proyecto? Proyecto { get; set; }
    }
}
