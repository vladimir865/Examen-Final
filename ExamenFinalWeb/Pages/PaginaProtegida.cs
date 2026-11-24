using System.Net.Http.Headers;
using System.Text.Json;
using Contratos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExamenFinalWeb.Pages
{
    // Clase "madre" de todas las páginas que necesitan login.
    // Así no repetimos el código del token en cada página.
    public abstract class PaginaProtegida : PageModel
    {
        protected readonly HttpClient Http;

        protected PaginaProtegida(IHttpClientFactory httpClientFactory)
        {
            Http = httpClientFactory.CreateClient("Api");
        }

        // ¿El que inició sesión es Administrador? (solo él puede registrar)
        public bool EsAdmin => Request.Cookies["rol"] == "Administrador";

        // Pone el token (el "pase") en los pedidos a la API. Devuelve false si no hay sesión.
        protected bool PonerToken()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
                return false;

            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        // Borra la sesión (por si el token venció) y manda al Login
        protected IActionResult IrAlLogin()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("rol");
            Response.Cookies.Delete("usuario");
            return RedirectToPage("/Login");
        }

        // Lee el error que mandó la API y lo pone debajo del campo que corresponde.
        // prefijo = nombre de la propiedad del formulario (ej: "Jefe" -> "Jefe.Nombre")
        protected async Task<string?> MostrarErrorDeApi(HttpResponseMessage respuesta, string prefijo)
        {
            var texto = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                using var json = JsonDocument.Parse(texto);

                // Caso 1: nuestro ErrorDto { campo, mensaje }
                if (json.RootElement.TryGetProperty("mensaje", out _))
                {
                    var error = JsonSerializer.Deserialize<ErrorDto>(texto, opciones)!;
                    if (!string.IsNullOrEmpty(error.Campo))
                    {
                        ModelState.AddModelError($"{prefijo}.{error.Campo}", error.Mensaje);
                        return null;
                    }
                    return error.Mensaje;
                }

                // Caso 2: errores de validación automáticos de [ApiController] { errors: { Campo: [...] } }
                if (json.RootElement.TryGetProperty("errors", out var errores))
                {
                    foreach (var campo in errores.EnumerateObject())
                    {
                        foreach (var msg in campo.Value.EnumerateArray())
                        {
                            ModelState.AddModelError($"{prefijo}.{campo.Name}", msg.GetString() ?? "Dato inválido");
                        }
                    }
                    return null;
                }
            }
            catch (JsonException)
            {
                // No era JSON: es un texto simple
            }

            return string.IsNullOrWhiteSpace(texto) ? "Ocurrió un error en la API." : texto.Trim('"');
        }
    }
}
