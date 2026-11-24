using System.ComponentModel.DataAnnotations;

namespace Contratos
{
    // Lo que se manda para iniciar sesión o registrarse
    public class LoginDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }

    // Lo que la API devuelve cuando el login sale bien
    public class LoginRespuestaDto
    {
        public string Token { get; set; } = string.Empty;   // el "pase" JWT
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
