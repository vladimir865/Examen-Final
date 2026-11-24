using System.Net;
using Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinalWeb.Pages
{
    public class IndexModel : PaginaProtegida
    {
        public IndexModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public int TotalJefes { get; private set; }
        public int JefesLibres { get; private set; }
        public int TotalProyectos { get; private set; }
        public int ProyectosEnProgreso { get; private set; }
        public string? MensajeError { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!PonerToken())
                return IrAlLogin();

            try
            {
                var jefes = await Http.GetFromJsonAsync<List<JefeRespuestaDto>>("api/Jefe") ?? new();
                var proyectos = await Http.GetFromJsonAsync<List<ProyectoRespuestaDto>>("api/Proyecto") ?? new();

                TotalJefes = jefes.Count;
                JefesLibres = jefes.Count(j => j.EstaLibre);
                TotalProyectos = proyectos.Count;
                ProyectosEnProgreso = proyectos.Count(p => p.Estado == "En progreso");
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
