# Refugio Animales

Sistema de gestión de un refugio de animales desarrollado como proyecto universitario para **SGBD II** y **Programación Visual**.

Permite administrar animales, adoptantes, solicitudes de adopción, tratamientos médicos y donaciones, con una base de datos MySQL que concentra la lógica de negocio en triggers, procedimientos y vistas.

## Stack

- C# / .NET 10
- Avalonia UI (MVVM con CommunityToolkit.Mvvm)
- MySQL 8.4 (WSL)
- MySqlConnector con SQL directo (sin ORM)

## Ubicación

```
C:\Users\Cafeina\Documents\RefugioAnimales
```

Desde WSL:

```
/mnt/c/Users/Cafeina/Documents/RefugioAnimales
```

## Estructura

```
RefugioAnimales.sln
src/Refugio.App/          Aplicación Avalonia
database/                 Scripts SQL (schema, triggers, procedimientos, seed, roles)
scripts/                  Utilidades para iniciar MySQL, crear la base, compilar y ejecutar
docs/er.md                Diagrama entidad-relación y notas de diseño
```

## Requisitos

- Windows con .NET SDK 10 (`C:\Program Files\dotnet`)
- WSL con MySQL 8.4 en ejecución
- Acceso a nuget.org para restaurar paquetes

## Base de datos

La base se llama `refugio` y la aplicación se conecta con el usuario `refugio_app`.

Para crearla desde cero:

```bash
cd /mnt/c/Users/Cafeina/Documents/RefugioAnimales
./scripts/setup-db.sh
```

El script solicita la contraseña de root de MySQL y ejecuta, en orden:

1. `database/01_schema.sql`
2. `database/02_triggers.sql`
3. `database/03_procedures_views.sql`
4. `database/04_seed.sql`
5. `database/05_roles.sql`

> Volver a ejecutarlo elimina y vuelve a crear las tablas con los datos de ejemplo.

También se puede ejecutar la prueba funcional:

```bash
mysql -u root -p < database/99_smoke_test.sql
```

La prueba crea datos temporales, aprueba una adopción, muestra el estado resultante y limpia los datos al final.

## Ejecución

```bash
cd /mnt/c/Users/Cafeina/Documents/RefugioAnimales

./scripts/start-db.sh   # inicia MySQL en WSL si hace falta
./scripts/build.sh      # compila con el dotnet.exe de Windows
./scripts/run.sh        # abre la aplicación en Windows
```

También se puede abrir `RefugioAnimales.sln` en Visual Studio y ejecutar el proyecto `Refugio.App`.

## Credenciales de prueba

| Rol | Email | Contraseña |
| --- | --- | --- |
| Administrador | `admin@refugio.local` | `Admin2026` |
| Veterinario | `vet@refugio.local` | `Vet2026` |
| Voluntario | `voluntario@refugio.local` | `Voluntario2026` |

Usuario de la aplicación en MySQL: `refugio_app` / `RefugioApp2026` (configurado en `src/Refugio.App/appsettings.json`).

## Funcionalidades

- Login con roles y contraseñas almacenadas con PBKDF2-SHA256.
- Panel con indicadores, adopciones por mes, donaciones mensuales y ranking de donantes.
- Catálogo de animales con filtros, edición y cambios de estado auditados.
- Gestión de adoptantes.
- Solicitudes de adopción con aprobación transaccional y rechazo automático de solicitudes rivales.
- Tratamientos médicos con permisos por rol.
- Registro de donaciones y usuarios (este último solo para administradores).

## Base de datos: puntos destacados

- **Triggers** para validar solicitudes, auditar cambios de estado y evitar transiciones inválidas.
- **Procedimiento `sp_aprobar_solicitud`** que aprueba una adopción y rechaza las solicitudes rivales dentro de una transacción con bloqueo de filas.
- **Vistas** `v_animales_catalogo`, `v_solicitudes_detalle` y `v_donaciones_mensuales`.
- **Procedimientos de reporte** `sp_adopciones_por_mes`, `sp_top_donantes` (con `RANK()`), `sp_tratamientos_pendientes` y `sp_resumen_dashboard`.
- **Roles de MySQL** de demostración en `database/05_roles.sql`.
- Claves foráneas, restricciones e índices sobre las columnas de búsqueda y filtrado.

Ver `docs/er.md` para el diagrama entidad-relación.

## Demo sugerida

1. Iniciar sesión como `admin@refugio.local`.
2. En **Solicitudes**, crear dos solicitudes pendientes para el mismo animal con adoptantes distintos.
3. Seleccionar una de las solicitudes y aprobarla.
4. Comprobar en **Animales** que el animal quedó en estado *Adoptado* y que la otra solicitud fue rechazada automáticamente con el motivo correspondiente.
5. Ver en el **Panel** cómo se actualizan los indicadores y en `historial_estados_animal` el registro de auditoría.

## Problemas comunes

- **No se puede conectar a la base**: verificar que MySQL esté activo con `service mysql status` y que el puerto `3306` responda.
- **Windows no alcanza el MySQL de WSL**: editar `src/Refugio.App/appsettings.json` y reemplazar `127.0.0.1` por la IP de WSL (`hostname -I`).
- **Restauración de NuGet**: el repositorio incluye `NuGet.Config` apuntando a nuget.org; se requiere conexión a internet la primera vez.
- **`dotnet` no encontrado en WSL**: la compilación se hace con `C:\Program Files\dotnet\dotnet.exe` mediante los scripts incluidos.

## Notas

- Las contraseñas del seed son de demostración; no deben usarse en un entorno real.
- El proyecto apunta a `net10.0` porque es el runtime instalado en Windows.
- El diagrama ER y las decisiones de normalización están en `docs/er.md`.
