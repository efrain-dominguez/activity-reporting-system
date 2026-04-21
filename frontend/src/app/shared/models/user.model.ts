export enum UserRole {
  Admin = 'Admin',
  PMO = 'PMO',
  Entity = 'Entity',
  Directorate = 'Directorate',
  Team = 'Team'
}

export interface User {
  id: string;
  entraObjectId: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  entityId?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}
