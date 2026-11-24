using System.Net;
using Contratos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamenFinalWeb.Pages.Proyectos
{
    public class CreateModel : PaginaProtegida
    {
        public CreateModel(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        [BindProperty]
        public ProyectoDto Proyecto { get; set; } = new();

        // Solo los jefes LIBRES (sin proyecto) para el desplegable
        public List<SelectListItem> JefesLibres { get; private set; } = new();
        public List<SelectListItem> ListaEstados { get; } =
            Estados.Todos.Select(e => new SelectListItem(e, e)).ToList();

        public string? MensajeError { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!PonerToken())
                return IrAlLogin();

            if (!EsAdmin)
            {
                TempData["Mensaje"] = "Solo el administrador puede registrar proyectos.";
                return RedirectToPage("Index");
            }

            return await CargarJefes() ? Page() : IrAlLogin();
        }

        // 2) POST Proyecto
        public async Task<IActionResult> OnPostAsync()
        {
            if (!PonerToken())
                return IrAlLogin();

            if (Proyecto.IdJefe == Guid.Empty)
                ModelState.AddModelError("Proyecto.IdJefe", "Selecciona un jefe.");

            if (!ModelState.IsValid)
            {
                await CargarJefes();
                return Page();
            }

            try
            {
                var respuesta = await Http.PostAsJsonAsync("api/Proyecto", Proyecto);

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                    return IrAlLogin();

                if (respuesta.StatusCode == HttpStatusCode.Forbidden)
                {
                    MensajeError = "Solo el administrador puede registrar proyectos.";
                    await CargarJefes();
                    return Page();
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    // Ej: nombre repetido (debajo de Nombre) o jefe ocupado (debajo de Jefe)
                    MensajeError = await MostrarErrorDeApi(respuesta, nameof(Proyecto));
                    await CargarJefes();
                    return Page();
                }
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API. ¿Está ejecutándose?";
                return Page();
            }

            TempData["Mensaje"] = $"Proyecto \"{Proyecto.Nombre.Trim()}\" registrado correctamente.";
            return RedirectToPage("Index");
        }

        // Trae los jefes de la API y deja solo los que no tienen proyecto
        private async Task<bool> CargarJefes()
        {
            try
            {
                var jefes = await Http.GetFromJsonAsync<List<JefeRespuestaDto>>("api/Jefe") ?? new();
                JefesLibres = jefes
                    .Where(j => j.EstaLibre)
                    .Select(j => new SelectListItem($"{j.Nombre} — {j.Cargo}", j.Id.ToString()))
                    .ToList();
                return true;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return false;
            }
            catch (HttpRequestException)
            {
                MensajeError = "No se pudo conectar con la API. ¿Está ejecutándose?";
                return true;
            }
        }
    }
}
