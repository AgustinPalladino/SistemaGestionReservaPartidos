export type TournamentStatus = 'Registration' | 'InProgress' | 'Finished';

export interface Tournament {
  id: number;
  organizerId: string;
  name: string;
  sportType: string;
  status: TournamentStatus;
  startDate: string;
  endDate: string;
}
