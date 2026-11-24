var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// URL de la API (está en appsettings.json)
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("Falta ApiSettings:BaseUrl en appsettings.json");

// Cliente HTTP con nombre "Api": todas las páginas lo usan para hablar con la API
builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
