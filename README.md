# Sistema de Gestión de Reservas y Partidos

Backend .NET 8 (Clean Architecture) + PostgreSQL + EF Core. Frontend React/Angular (pendiente).

## Estructura

```
src/
├── Domain/                    # Entidades y enums
├── Application/               # Lógica de negocio, interfaces (puertos)
│   ├── Interfaces/
│   │   └── Persistence/
│   │       ├── Repositories/
│   │       ├── IScheduleConflictCoordinator.cs
│   │       └── IScheduleConflictScope.cs
│   └── Services/
│       ├── Reservations/      # Solapamiento, opciones de horario
│       └── Matches/           # Validaciones de partidos
├── Infrastructure/            # EF Core, repositorios, bloqueos PostgreSQL
│   ├── Persistence/
│   └── Repositories/
└── SistemaGestion.API/        # Controllers + Models (presentación)
```

## Ejecutar la API

```powershell
cd src\SistemaGestion.API
dotnet run
```

Swagger: `http://localhost:5080/swagger`

CORS habilitado para `localhost:3000`, `5173` y `4200`.

## Migraciones EF Core

```powershell
dotnet ef migrations add InitialCreate `
  --project src\Infrastructure\Infrastructure.csproj `
  --startup-project src\SistemaGestion.API\SistemaGestion.API.csproj `
  --output-dir Persistence\Migrations

dotnet ef database update `
  --project src\Infrastructure\Infrastructure.csproj `
  --startup-project src\SistemaGestion.API\SistemaGestion.API.csproj
```

## Endpoints principales

| Recurso | Ruta base |
|---------|-----------|
| Canchas | `GET/POST /api/fields`, `PUT /api/fields/{id}` |
| Usuarios | `GET/POST /api/users` (sin exponer password) |
| Reservas | `GET/POST /api/reservations`, `POST .../confirm`, `.../cancel` |
| Torneos | `GET/POST/PUT /api/tournaments` |
| Equipos | `GET/POST /api/tournaments/{id}/teams` |
| Partidos | `GET/POST /api/tournaments/{id}/matches`, `PATCH .../score` |

## Capas y responsabilidades

- **Application**: reglas de negocio (`ReservationScheduleService`, `ScheduleOverlapCalculator`, `MatchValidation`).
- **Infrastructure**: acceso a datos, transacciones, `pg_advisory_xact_lock`.
- **API**: solo orquesta HTTP; inyecta interfaces de Application.
