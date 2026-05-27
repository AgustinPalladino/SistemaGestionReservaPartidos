# Sistema de Gestión de Reservas y Partidos

Monorepo: **backend** (.NET 8 + PostgreSQL) y **frontend** (React/Angular).

## Estructura del repositorio

```
SistemaGestionReservaPartidos/
├── backend/
│   ├── SistemaGestionReservaPartidos.sln
│   └── src/
│       ├── Domain/
│       ├── Application/
│       ├── Infrastructure/
│       └── SistemaGestion.API/
└── frontend/
```

No hace falta cambiar namespaces ni referencias en el código: los `.csproj` siguen enlazándose con rutas relativas (`..\Domain\`, etc.) dentro de `backend/src`.

## Backend — ejecutar la API

Desde la carpeta `backend`:

```powershell
cd backend\src\SistemaGestion.API
dotnet run
```

O abrir `backend\SistemaGestionReservaPartidos.sln` en Visual Studio / Rider.

Swagger: `http://localhost:5080/swagger`

## Backend — migraciones EF Core

Ejecutar desde la raíz del repo o desde `backend`:

```powershell
dotnet ef migrations add InitialCreate `
  --project backend\src\Infrastructure\Infrastructure.csproj `
  --startup-project backend\src\SistemaGestion.API\SistemaGestion.API.csproj `
  --output-dir Persistence\Migrations

dotnet ef database update `
  --project backend\src\Infrastructure\Infrastructure.csproj `
  --startup-project backend\src\SistemaGestion.API\SistemaGestion.API.csproj
```

## Endpoints principales

| Recurso | Ruta base |
|---------|-----------|
| Canchas | `GET/POST /api/fields`, `PUT /api/fields/{id}` |
| Usuarios | `GET/POST /api/users` |
| Reservas | `GET/POST /api/reservations`, `POST .../confirm`, `.../cancel` |
| Torneos | `GET/POST/PUT /api/tournaments` |
| Equipos | `GET/POST /api/tournaments/{id}/teams` |
| Partidos | `GET/POST /api/tournaments/{id}/matches`, `PATCH .../score` |

## Capas (Clean Architecture)

- **Application**: lógica de negocio y puertos (`Interfaces/Persistence/Repositories/`).
- **Infrastructure**: EF Core, repositorios, bloqueos PostgreSQL.
- **SistemaGestion.API**: controllers y modelos HTTP.
