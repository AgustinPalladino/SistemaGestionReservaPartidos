import axios from 'axios';
import type { Field } from '../types/field';
import type { Reservation } from '../types/reservation';
import type { Tournament, TournamentStatus } from '../types/tournament';
import type { Team } from '../types/team';
import type { Match, MatchStatus } from '../types/match';
import type { User, UserRole } from '../types/user';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080/api',
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface CreateFieldPayload {
  name: string;
  sportType: string;
  pricePerHour: number;
}

export interface UpdateFieldPayload {
  name: string;
  sportType: string;
  pricePerHour: number;
  isActive: boolean;
}

export interface CreateReservationPayload {
  userId: string;
  fieldId: number;
  date: string;
  startTime: string;
  endTime: string;
  totalAmount: number;
}

export interface CreateTournamentPayload {
  organizerId: string;
  name: string;
  sportType: string;
  status: TournamentStatus;
  startDate: string;
  endDate: string;
}

export interface UpdateTournamentPayload {
  name: string;
  sportType: string;
  status: TournamentStatus;
  startDate: string;
  endDate: string;
}

export interface CreateTeamPayload {
  tournamentId: number;
  name: string;
  captainId?: string;
}

export interface CreateMatchPayload {
  tournamentId: number;
  homeTeamId: number;
  awayTeamId: number;
  fieldId?: number;
  scheduledAt: string;
  status: MatchStatus;
}

export interface CreateUserPayload {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: UserRole;
}

export const fieldService = {
  getAll: (activeOnly = true) =>
    api.get<Field[]>('/fields', { params: { activeOnly } }).then((r) => r.data),
  getById: (id: number) => api.get<Field>(`/fields/${id}`).then((r) => r.data),
  create: (data: CreateFieldPayload) => api.post<Field>('/fields', data).then((r) => r.data),
  update: (id: number, data: UpdateFieldPayload) =>
    api.put(`/fields/${id}`, data).then((r) => r.data),
  deactivate: async (id: number) => {
    const field = await api.get<Field>(`/fields/${id}`).then((r) => r.data);
    return api
      .put<Field>(`/fields/${id}`, { ...field, isActive: false })
      .then((r) => r.data);
  },
};

export const reservationService = {
  getAll: () => api.get<Reservation[]>('/reservations').then((r) => r.data),
  getById: (id: string) => api.get<Reservation>(`/reservations/${id}`).then((r) => r.data),
  create: (data: CreateReservationPayload) =>
    api.post<Reservation>('/reservations', data).then((r) => r.data),
  confirm: (id: string) => api.post(`/reservations/${id}/confirm`),
  cancel: (id: string) => api.post(`/reservations/${id}/cancel`),
};

export const tournamentService = {
  getAll: () => api.get<Tournament[]>('/tournaments').then((r) => r.data),
  getById: (id: number) => api.get<Tournament>(`/tournaments/${id}`).then((r) => r.data),
  create: (data: CreateTournamentPayload) =>
    api.post<Tournament>('/tournaments', data).then((r) => r.data),
  update: (id: number, data: UpdateTournamentPayload) =>
    api.put(`/tournaments/${id}`, data).then((r) => r.data),
  updateStatus: (id: number, status: TournamentStatus) =>
    tournamentService.getById(id).then((tournament) =>
      api.put<Tournament>(`/tournaments/${id}`, { ...tournament, status }).then((r) => r.data),
    ),
};

export const teamService = {
  getByTournament: (tournamentId: number) =>
    api.get<Team[]>(`/tournaments/${tournamentId}/teams`).then((r) => r.data),
  create: (data: CreateTeamPayload) =>
    api
      .post<Team>(`/tournaments/${data.tournamentId}/teams`, data)
      .then((r) => r.data),
};

export const matchService = {
  getByTournament: (tournamentId: number) =>
    api.get<Match[]>(`/tournaments/${tournamentId}/matches`).then((r) => r.data),
  updateScore: (
    tournamentId: number,
    id: number,
    homeScore: number,
    awayScore: number,
    status: MatchStatus = 'Finished',
  ) =>
    api
      .patch(`/tournaments/${tournamentId}/matches/${id}/score`, {
        homeScore,
        awayScore,
        status,
      })
      .then((r) => r.data),
};

export const userService = {
  getAll: () => api.get<User[]>('/users').then((r) => r.data),
  getById: (id: string) => api.get<User>(`/users/${id}`).then((r) => r.data),
  create: (data: CreateUserPayload) => api.post<User>('/users', data).then((r) => r.data),
};

export default api;
