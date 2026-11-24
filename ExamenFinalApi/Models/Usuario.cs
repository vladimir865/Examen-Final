namespace ExamenFinalApi.Models
{
    // Tabla "usuarios": va aparte, solo sirve para iniciar sesión
    public class Usuario
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;   // nunca la contraseña en texto plano
        public string Rol { get; set; } = "Usuario";               // "Administrador" o "Usuario"
    }
}
