using System.Net;
using Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinalWeb.Pages.Proyectos
{
    // 3) Buscar proyecto por ID
    public class BuscarModel : PaginaProtegida
    {
        public BuscarModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }

        public ProyectoRespuestaDto? Proyecto { get; private set; }
        public string? MensajeError { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!PonerToken())
                return IrAlLogin();

            // Todavía no escribió nada: solo muestra el buscador
            if (string.IsNullOrWhiteSpace(Id))
                return Page();

            if (!Guid.TryParse(Id.Trim(), out var guid))
            {
                MensajeError = "El ID no tiene un formato válido. Debe ser como: 3f2a9c1e-5b7d-4e8a-9c21-7d4e5f6a8b90";
                return Page();
            }

            try
            {
                var respuesta = await Http.GetAsync($"api/Proyecto/{guid}");

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                    return IrAlLogin();

                if (respuesta.StatusCode == HttpStatusCode.NotFound)
                {
                    MensajeError = "No existe un proyecto con ese ID.";
                    return Page();
                }

                respuesta.EnsureSuccessStatusCode();
                Proyecto = await respuesta.Content.ReadFromJsonAsync<ProyectoRespuestaDto>();
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API. ¿Está ejecutándose?";
            }

            return Page();
        }
    }
}
