using System.ComponentModel.DataAnnotations;

namespace ExamenFinalApi.Models
{
    // Tabla "proyectos"
    public class Proyecto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;     // único: no se repite

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = "Pendiente";       // Pendiente, En progreso o Finalizado

        // Clave foránea al jefe. Es ÚNICA: un jefe no puede tener 2 proyectos.
        public Guid IdJefe { get; set; }
        public Jefe? Jefe { get; set; }
    }
}
