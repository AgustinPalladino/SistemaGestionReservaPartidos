# Sistema de Gestión de Reservas y Partidos

Monorepo: **backend** (.NET 8 + PostgreSQL) y **frontend** (React + Vite + Material UI).

---

## Sobre el Proyecto y Metodología

Este es mi **primer proyecto integral Full Stack** independiente, desarrollado con el objetivo de construir una aplicación de nivel profesional aplicando las mejores prácticas de la industria en cada etapa del desarrollo.

Asumí la responsabilidad completa del ciclo de vida del software:

- **Desarrollo Backend & Frontend:** Diseñé una arquitectura limpia y escalable en el backend con .NET 8 y PostgreSQL, integrándolo con una interfaz reactiva y amigable desarrollada en React + Vite.
- **Gestión Ágil con Jira (Scrum):** Para organizar los requerimientos de forma profesional como se trabaja en empresas de tecnología, utilicé **Jira** para estructurar el desarrollo mediante el marco **Scrum**. Dividí las funcionalidades en Sprints, Historias de Usuario, tareas con Criterios de Aceptación y seguimiento de estados (To Do, In Progress, Done).

> 🔗 **Tablero de Jira:** Puedes revisar el backlog, la planificación de sprints y la gestión de tareas en tiempo real desde [Tablero - SCRUM Sprint 0 - Jira](https://palladinospace.atlassian.net/jira/software/projects/SCRUM/boards/1).

---

## 📂 Estructura del repositorio

```text
SistemaGestionReservaPartidos/
├── backend/
│   ├── SistemaGestionReservaPartidos.sln
│   └── src/
│       ├── Domain/             # Entidades de dominio y reglas de negocio
│       ├── Application/        # Casos de uso e interfaces
│       ├── Infrastructure/     # EF Core, repositorios y persistencia
│       └── SistemaGestion.API/ # Controladores y endpoints REST
└── frontend/                   # Aplicación en React + Vite + MUI
```

No hace falta cambiar namespaces ni referencias en el código: los `.csproj` siguen enlazándose con rutas relativas (`..\Domain\`, etc.) dentro de `backend/src`.

## Backend — ejecutar la API

Desde la carpeta `backend`:

```powershell
cd backend
dotnet restore
dotnet run --project src/SistemaGestion.API/SistemaGestion.API.csproj
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

## Frontend - Ejecutar:
cd frontend
npm run dev


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
