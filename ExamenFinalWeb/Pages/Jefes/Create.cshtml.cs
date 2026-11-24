using System.Net;
using Contratos;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinalWeb.Pages.Jefes
{
    public class CreateModel : PaginaProtegida
    {
        private readonly IWebHostEnvironment _env;

        public CreateModel(IHttpClientFactory httpClientFactory, IWebHostEnvironment env) : base(httpClientFactory)
        {
            _env = env;
        }

        [BindProperty]
        public JefeDto Jefe { get; set; } = new();

        // La foto elegida en el formulario (solo la usa la Web)
        [BindProperty]
        public IFormFile? Foto { get; set; }

        public string? MensajeError { get; private set; }

        public IActionResult OnGet()
        {
            if (Request.Cookies["token"] == null)
                return IrAlLogin();

            if (!EsAdmin)
            {
                TempData["Mensaje"] = "Solo el administrador puede registrar jefes.";
                return RedirectToPage("Index");
            }
            return Page();
        }

        // 1) POST Jefe
        public async Task<IActionResult> OnPostAsync()
        {
            if (!PonerToken())
                return IrAlLogin();

            if (Foto != null && !Foto.ContentType.StartsWith("image/"))
                ModelState.AddModelError(nameof(Foto), "El archivo debe ser una imagen (jpg, png...).");

            if (!ModelState.IsValid)
                return Page();

            // 1. La WEB guarda la foto en su carpeta wwwroot/imagenes
            string? rutaFisica = null;
            if (Foto != null && Foto.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "imagenes");
                Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(Foto.FileName);
                rutaFisica = Path.Combine(carpeta, nombreArchivo);
                using (var stream = new FileStream(rutaFisica, FileMode.Create))
                {
                    await Foto.CopyToAsync(stream);
                }

                Jefe.Foto = "imagenes/" + nombreArchivo;   // solo la RUTA viaja a la API
            }

            // 2. Manda el jefe a la API como JSON
            try
            {
                var respuesta = await Http.PostAsJsonAsync("api/Jefe", Jefe);

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                {
                    BorrarFoto(rutaFisica);
                    return IrAlLogin();
                }

                if (respuesta.StatusCode == HttpStatusCode.Forbidden)
                {
                    BorrarFoto(rutaFisica);
                    MensajeError = "Solo el administrador puede registrar jefes.";
                    return Page();
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    // Ej: "Ya existe un jefe con ese nombre." -> debajo del campo Nombre
                    BorrarFoto(rutaFisica);
                    MensajeError = await MostrarErrorDeApi(respuesta, nameof(Jefe));
                    return Page();
                }
            }
            catch (HttpRequestException)
            {
                BorrarFoto(rutaFisica);
                MensajeError = "No se pudo conectar con la API. ¿Está ejecutándose?";
                return Page();
            }

            TempData["Mensaje"] = $"Jefe \"{Jefe.Nombre.Trim()}\" registrado correctamente.";
            return RedirectToPage("Index");
        }

        // Si la API no guardó el jefe, no dejamos la foto suelta en la carpeta
        private static void BorrarFoto(string? rutaFisica)
        {
            if (rutaFisica != null && System.IO.File.Exists(rutaFisica))
                System.IO.File.Delete(rutaFisica);
        }
    }
}
