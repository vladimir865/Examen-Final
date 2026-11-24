using Contratos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExamenFinalWeb.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _http;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        [BindProperty]
        public LoginDto Datos { get; set; } = new();

        public string? MensajeError { get; private set; }

        public IActionResult OnGet()
        {
            // Si ya inició sesión, no tiene sentido mostrar el login
            if (Request.Cookies["token"] != null)
                return RedirectToPage("/Index");
            return Page();
        }

        // 4) Iniciar sesión: pide el token a la API y lo guarda en una cookie
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/Auth/login", Datos);

                if (!respuesta.IsSuccessStatusCode)
                {
                    MensajeError = "Usuario o contraseña incorrectos.";
                    return Page();
                }

                var sesion = await respuesta.Content.ReadFromJsonAsync<LoginRespuestaDto>();

                // Cookies que duran lo mismo que el token (2 horas)
                var opciones = new CookieOptions { HttpOnly = true, Expires = DateTimeOffset.Now.AddHours(2) };
                Response.Cookies.Append("token", sesion!.Token, opciones);
                Response.Cookies.Append("rol", sesion.Rol, opciones);
                Response.Cookies.Append("usuario", sesion.NombreUsuario, opciones);

                return RedirectToPage("/Index");
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API. ¿Está ejecutándose?";
                return Page();
            }
        }

        // Cerrar sesión: borra las cookies
        public IActionResult OnGetSalir()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("rol");
            Response.Cookies.Delete("usuario");
            return RedirectToPage("/Login");
        }
    }
}
