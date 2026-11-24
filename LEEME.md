# Examen Final – Ejercicio A (Proyecto y Jefe)

Solución en 3 capas con .NET 10:

| Proyecto | Qué es |
| --- | --- |
| `ExamenFinalApi` | Web API: Models, Data (DbContext), Interfaces, Services, Controllers, Factories (semillas) |
| `Contratos` | DTOs con validaciones, usados por la API y la Web |
| `ExamenFinalWeb` | Frontend Razor Pages, color verde |

## Cómo ejecutarlo

1. Abre `ExamenFinalApi.slnx` en Visual Studio. La primera vez descarga los paquetes NuGet.
2. **(Recomendado) Crea la migración** en la Consola del Administrador de paquetes:
   ```
   Add-Migration Inicial -Project ExamenFinalApi -StartupProject ExamenFinalApi
   ```
   No hace falta `Update-Database`: la API aplica la migración sola al arrancar.
   Si no creas la migración, la API igual crea las tablas (con `EnsureCreated`).
   Si primero la ejecutas SIN migración y después agregas una, borra la base
   `ExamenFinalDb` antes de volver a ejecutar.
3. Clic derecho en la solución → **Configurar proyectos de inicio** → **Varios proyectos de inicio**:
   `ExamenFinalApi` = Iniciar, `ExamenFinalWeb` = Iniciar.
4. Ejecuta (F5). La base `ExamenFinalDb` se crea en `(localdb)\MSSQLLocalDB` con datos de prueba:
   - Usuario **admin / 123456** (rol Administrador)
   - 10 jefes y 10 proyectos (cada jefe ya tiene su proyecto: para crear un proyecto nuevo,
     primero registra un jefe nuevo)
5. Swagger: http://localhost:5100/swagger · Web: http://localhost:5200

## Lo que pide la pizarra

| Pedido | Dónde está |
| --- | --- |
| 1) POST Jefe | `POST api/Jefe` (JSON) · Web: Jefes → Nuevo jefe |
| 2) POST Proyecto | `POST api/Proyecto` · Web: Proyectos → Nuevo proyecto |
| 3) Buscar proyecto por ID | `GET api/Proyecto/{id}` · Web: Buscar por ID |
| 4) Iniciar sesión con token | `POST api/Auth/login` devuelve un JWT · Web: Login |
| 5) Frontend verde | Razor Pages con tema verde |

### Reglas

| Regla | Cómo se cumple |
| --- | --- |
| Un jefe no tiene el mismo nombre que otro | Índice único en `jefes.Nombre` + validación en `JefeService` (409) |
| Dos proyectos no pueden tener el mismo nombre | Índice único en `proyectos.Nombre` + validación en `ProyectoService` (409) |
| Un jefe solo puede estar a cargo de 1 proyecto | Relación 1 a 1 (`IdJefe` único) + validación en `ProyectoService` (409); en la Web solo se ofrecen jefes libres |

## Probar en Swagger

1. `POST /api/Auth/login` con `{ "nombreUsuario": "admin", "password": "123456" }` → copia el `token`.
2. Botón 🔒 **Authorize** → pega el token (sin escribir "Bearer").
3. Ya puedes usar `GET/POST api/Jefe` y `GET/POST api/Proyecto`. Sin token, la API responde **401**.

## Roles

- **Administrador**: registra jefes y proyectos.
- **Usuario** (quien se registra en `POST api/Auth/registrar`): solo puede ver. Si intenta registrar, la API responde **403**.

## Extras

- Foto del jefe: la Web la guarda en su carpeta `ExamenFinalWeb/wwwroot/imagenes` y manda a la API solo la ruta (campo `foto` del JSON). GLightbox para verla en grande.
- Semillas con Bogus en `Factories/SemillasFactory.cs`.
- Contraseñas encriptadas con `PasswordHasher`.
- Buscadores en las listas y botón para copiar el ID de un proyecto.
