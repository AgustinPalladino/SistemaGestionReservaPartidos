export interface Team {
  id: number;
  tournamentId: number;
  name: string;
  captainId?: string;
  points: number;
  matchesPlayed: number;
}
