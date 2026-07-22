export interface CalendarEventDto {
  id: string;
  date: string;
  startTime: string;
  endTime: string;
  type: string;
  title?: string;
}

export interface AvailableHourDto {
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}
