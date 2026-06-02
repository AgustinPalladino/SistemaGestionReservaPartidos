# Frontend — SGR Partidos

React 18+ · TypeScript · Vite · MUI · Axios · React Router v6/v7.

## Estructura (`src/`)

```
src/
├── components/       # Layout, Navbar, Sidebar (shell global)
├── features/         # Módulos por contexto (espejo de Application en backend)
│   ├── auth/
│   ├── dashboard/
│   ├── fields/
│   ├── reservations/
│   ├── tournaments/
│   └── teams/
├── services/         # Capa HTTP (axios + interceptores)
├── theme/            # Tema MUI
└── types/            # Contratos TS alineados con entidades C#
```

## Requisitos

- Node.js 20+
- API backend en ejecución (`http://localhost:5080`)

## Instalación

Si clonás el repo sin `node_modules`:

```bash
cd frontend
npm install
```

### Dependencias principales (proyecto Vite + React + TS existente)

```bash
npm install @mui/material @emotion/react @emotion/styled @mui/icons-material
npm install axios
npm install react-router-dom
```

Opcional (date pickers para reservas más adelante):

```bash
npm install @mui/x-date-pickers dayjs
```

## Variables de entorno

Copiá `.env.example` a `.env`:

```bash
cp .env.example .env
```

| Variable | Descripción |
|----------|-------------|
| `VITE_API_BASE_URL` | Base de la API (default `http://localhost:5080/api`) |

## Scripts

```bash
npm run dev      # http://localhost:5173
npm run build
npm run preview
```

## Rutas

| Ruta | Página |
|------|--------|
| `/login` | Login (sin Layout) |
| `/` | Dashboard |
| `/fields` | Canchas |
| `/reservations` | Reservas |
| `/tournaments` | Torneos |
| `/teams` | Equipos |

## Servicios HTTP (`services/api.ts`)

- Instancia Axios con interceptor JWT (`localStorage.token`).
- Servicios por entidad: `fieldService`, `reservationService`, `tournamentService`, `teamService`, `matchService`, `userService`.
- Rutas alineadas con `SistemaGestion.API` (p. ej. `POST /reservations/{id}/cancel`, equipos bajo `/tournaments/{id}/teams`).

## Notas

- **MUI**: el proyecto usa MUI v9 (compatible con la API de tema de v5).
- **Auth**: JWT preparado en interceptor; login es placeholder hasta implementar endpoint en backend.
- **Estado**: sin Redux; `useState` / contexto local por ahora.
- **CORS**: el backend ya permite `localhost:5173`.

## Próximos pasos sugeridos

1. Pantalla de Fields con tabla MUI y formulario.
2. Flujo de reservas con selector de fecha/hora.
3. Login real cuando exista endpoint JWT en la API.
