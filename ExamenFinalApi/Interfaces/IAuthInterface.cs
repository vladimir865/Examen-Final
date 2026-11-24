using Contratos;

namespace ExamenFinalApi.Interfaces
{
    public interface IAuthInterface
    {
        // Devuelve null si salió bien, o el mensaje de error
        string? Registrar(LoginDto dto);

        // Devuelve el token, o null si el usuario o la contraseña están mal
        LoginRespuestaDto? Login(LoginDto dto);
    }
}
