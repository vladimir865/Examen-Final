using System.ComponentModel.DataAnnotations;

namespace Contratos
{
    // Lo que se manda para REGISTRAR un jefe (JSON)
    public class JefeDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Cargo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Escribe un correo válido (ej: juan@empresa.com)")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Correo { get; set; } = string.Empty;

        // Ruta de la foto (opcional). La Web guarda la imagen y aquí solo manda la ruta,
        // ej: "imagenes/abc.jpg"
        [StringLength(300, ErrorMessage = "Máximo 300 caracteres")]
        public string? Foto { get; set; }
    }

    // Lo que la API DEVUELVE de un jefe
    public class JefeRespuestaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Foto { get; set; }                 // ej: imagenes/abc.jpg

        // Proyecto que tiene a cargo (null = está libre)
        public string? ProyectoNombre { get; set; }
        public bool EstaLibre => ProyectoNombre == null;
    }
}
