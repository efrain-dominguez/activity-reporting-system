import { Frequency } from "./frequency.model";


export interface TrackingRequest {
  id: string;
  title: string;
  description: string;
  goalType: string;
  createdByUserId: string;
  targetEntityIds: string[];
  startDate: Date;
  dueDate: Date;
  frequency?: Frequency ;
  status: RequestStatus;
  isRecurring: boolean;
  createdAt: Date;
  updatedAt: Date;
}

// Status enum matching backend
export enum RequestStatus {
  Draft = 'Draft',
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Overdue = 'Overdue'
}

// DTO for creating new requests
export interface CreateTrackingRequestDto {
  title: string;
  description: string;
  goalType: string;
  targetEntityIds: string[];
  startDate: Date;
  dueDate: Date;
  frequency?: Frequency;
  isRecurring: boolean;
}

// DTO for updating requests
export interface UpdateTrackingRequestDto {
  title?: string;
  description?: string;
  goalType?: string;
  targetEntityIds?: string[];
  startDate?: Date;
  dueDate?: Date;
  frequency?: Frequency;
  isRecurring?: boolean;
  status?: RequestStatus;
}

// Helper function to check if overdue
export function isRequestOverdue(request: TrackingRequest): boolean {
  return new Date() > new Date(request.dueDate) && request.status !== RequestStatus.Completed;
}

// Helper function to get status display class
export function getRequestStatusClass(status: RequestStatus): string {
  switch(status) {
    case RequestStatus.Draft:
      return 'badge bg-slate-150 text-slate-800 dark:bg-navy-500 dark:text-navy-100';
    case RequestStatus.Pending:
      return 'badge bg-warning/10 text-warning dark:bg-warning/15';
    case RequestStatus.InProgress:
      return 'badge bg-info/10 text-info dark:bg-info/15';
    case RequestStatus.Completed:
      return 'badge bg-success/10 text-success dark:bg-success/15';
    case RequestStatus.Overdue:
      return 'badge bg-error/10 text-error dark:bg-error/15';
    default:
      return 'badge bg-slate-150 text-slate-800 dark:bg-navy-500 dark:text-navy-100';
  }
}
