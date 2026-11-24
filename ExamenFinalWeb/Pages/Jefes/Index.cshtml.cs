using System.Net;
using Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinalWeb.Pages.Jefes
{
    public class IndexModel : PaginaProtegida
    {
        public IndexModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public List<JefeRespuestaDto> Jefes { get; private set; } = new();
        public string? MensajeError { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!PonerToken())
                return IrAlLogin();

            try
            {
                Jefes = await Http.GetFromJsonAsync<List<JefeRespuestaDto>>("api/Jefe") ?? new();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return IrAlLogin();
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API. ¿Está ejecutándose?";
            }

            return Page();
        }
    }
}
