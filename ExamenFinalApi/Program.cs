using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ExamenFinalApi.Data;
using ExamenFinalApi.Factories;
using ExamenFinalApi.Interfaces;
using ExamenFinalApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Base de datos ----------
var conexion = builder.Configuration.GetConnectionString("ConexionDb");
builder.Services.AddDbContext<BaseDatos>(options => options.UseSqlServer(conexion));

// ---------- Servicios (Interfaz -> Clase que la implementa) ----------
builder.Services.AddScoped<IJefeInterface, JefeService>();
builder.Services.AddScoped<IProyectoInterface, ProyectoService>();
builder.Services.AddScoped<IAuthInterface, AuthService>();

// ---------- JWT: el guardia que revisa el token ----------
var clave = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Clave"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(clave)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---------- Swagger con el botón "Authorize" (candado) ----------
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega aquí tu token (sin escribir 'Bearer'). Lo obtienes en POST /api/Auth/login"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ---------- Crear la base + semillas al arrancar ----------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BaseDatos>();

    if (context.Database.GetMigrations().Any())
        context.Database.Migrate();        // aplica las migraciones (carpeta Migrations)
    else
        context.Database.EnsureCreated();  // si aún no hiciste Add-Migration, crea las tablas igual

    SemillasFactory.Sembrar(context);      // admin/123456, 10 jefes y 10 proyectos
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();   // 1ro: ¿quién eres? (lee el token)
app.UseAuthorization();    // 2do: ¿puedes pasar?

app.MapControllers();

app.Run();
