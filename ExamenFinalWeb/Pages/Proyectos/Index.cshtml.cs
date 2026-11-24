using System.Net;
using Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinalWeb.Pages.Proyectos
{
    public class IndexModel : PaginaProtegida
    {
        public IndexModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public List<ProyectoRespuestaDto> Proyectos { get; private set; } = new();
        public string? MensajeError { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!PonerToken())
                return IrAlLogin();

            try
            {
                Proyectos = await Http.GetFromJsonAsync<List<ProyectoRespuestaDto>>("api/Proyecto") ?? new();
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

        // Color del estado
        public static string ColorEstado(string estado) => estado switch
        {
            "Finalizado" => "bg-success",
            "En progreso" => "bg-warning text-dark",
            _ => "bg-secondary"
        };
    }
}
