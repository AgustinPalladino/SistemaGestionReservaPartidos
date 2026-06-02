export type ReservationStatus = 'Pending' | 'Confirmed' | 'Cancelled';

export interface Reservation {
  id: string;
  userId: string;
  fieldId: number;
  date: string;
  startTime: string;
  endTime: string;
  status: ReservationStatus;
  totalAmount: number;
  createdAt: string;
}
