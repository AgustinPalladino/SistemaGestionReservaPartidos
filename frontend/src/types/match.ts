export type MatchStatus = 'Scheduled' | 'InProgress' | 'Finished' | 'Suspended';

export interface Match {
  id: number;
  tournamentId: number;
  homeTeamId: number;
  awayTeamId: number;
  fieldId?: number;
  scheduledAt: string;
  homeScore?: number;
  awayScore?: number;
  status: MatchStatus;
}
