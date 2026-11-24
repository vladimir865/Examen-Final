using System.Globalization;
using System.Text;
using Bogus;
using Microsoft.AspNetCore.Identity;
using ExamenFinalApi.Data;
using ExamenFinalApi.Models;

namespace ExamenFinalApi.Factories
{
    // Llena la base con datos de prueba la primera vez (si está vacía)
    public static class SemillasFactory
    {
        public static void Sembrar(BaseDatos context)
        {
            SembrarAdmin(context);

            // Si ya hay jefes, no hace nada (así no se pisa lo que registraste)
            if (context.Jefes.Any())
                return;

            Randomizer.Seed = new Random(2026);   // siempre los mismos datos

            // ---------- 10 JEFES (nombres únicos) ----------
            var cargos = new[]
            {
                "Jefe de Proyecto", "Líder Técnico", "Gerente de TI", "Coordinador de Desarrollo",
                "Jefe de Infraestructura", "Arquitecto de Software", "Scrum Master", "Jefe de Calidad"
            };

            var nombresUsados = new HashSet<string>();
            var faker = new Faker("es");
            var jefes = new List<Jefe>();

            while (jefes.Count < 10)
            {
                var nombre = faker.Name.FirstName();
                var apellido = faker.Name.LastName();
                var completo = $"{nombre} {apellido}";
                if (!nombresUsados.Add(completo))
                    continue;   // nombre repetido: probar otro

                jefes.Add(new Jefe
                {
                    Id = Guid.NewGuid(),
                    Nombre = completo,
                    Cargo = faker.PickRandom(cargos),
                    Correo = $"{SinTildes(nombre)}.{SinTildes(apellido)}@empresa.com".ToLower().Replace(" ", ""),
                    Foto = null
                });
            }
            context.Jefes.AddRange(jefes);

            // ---------- 10 PROYECTOS (uno por cada jefe) ----------
            var sistemas = new[]
            {
                "Sistema de Ventas", "Sistema de Inventario", "Portal Web", "App Móvil", "Sistema de Reservas",
                "Plataforma de Cursos", "Sistema de Facturación", "Control de Asistencia", "Sistema Contable",
                "Gestión de Clientes", "Sistema de Biblioteca", "Tienda Online"
            };
            var lugares = new[]
            {
                "Tarija", "La Paz", "Santa Cruz", "Cochabamba", "Sucre", "Potosí", "Oruro", "Beni", "Pando"
            };
            var estados = new[] { "Pendiente", "En progreso", "Finalizado" };

            var proyectosUsados = new HashSet<string>();
            var proyectos = new List<Proyecto>();

            foreach (var jefe in jefes)
            {
                string nombreProyecto;
                do
                {
                    nombreProyecto = $"{faker.PickRandom(sistemas)} {faker.PickRandom(lugares)}";
                } while (!proyectosUsados.Add(nombreProyecto));

                proyectos.Add(new Proyecto
                {
                    Id = Guid.NewGuid(),
                    Nombre = nombreProyecto,
                    Descripcion = $"Desarrollo del {nombreProyecto.ToLower()} para {faker.Company.CompanyName()}.",
                    Estado = faker.PickRandom(estados),
                    IdJefe = jefe.Id          // cada jefe tiene un solo proyecto
                });
            }
            context.Proyectos.AddRange(proyectos);

            context.SaveChanges();
        }

        // Usuario administrador por defecto: admin / 123456
        private static void SembrarAdmin(BaseDatos context)
        {
            if (context.Usuarios.Any(u => u.NombreUsuario == "admin"))
                return;

            var admin = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = "admin",
                Rol = "Administrador"
            };
            admin.PasswordHash = new PasswordHasher<Usuario>().HashPassword(admin, "123456");

            context.Usuarios.Add(admin);
            context.SaveChanges();
        }

        // "José Pérez" -> "Jose Perez" (para armar correos)
        private static string SinTildes(string texto)
        {
            var normalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalizado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
