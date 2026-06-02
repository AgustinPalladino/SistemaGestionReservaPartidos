export type UserRole = 'Admin' | 'Client' | 'Organizer';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  createdAt: string;
}
