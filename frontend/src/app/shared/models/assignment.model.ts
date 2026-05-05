export interface RequestAssignment {
  id: string;
  requestId: string;
  assignedToEntityId: string;
  assignedToUserId?: string;
  assignedByUserId: string;
  delegatedFromUserId?: string;
  status: AssignmentStatus;
  noProgressReason?: string;
  extensionRequested: boolean;
  extensionRequestedDate?: Date;
  extensionGranted: boolean;
  extensionGrantedDate?: Date;
  newDueDate?: Date;
  submittedAt?: Date;
  reviewedAt?: Date;
  createdAt: Date;
  updatedAt: Date;
}

export enum AssignmentStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Overdue = 'Overdue',
  Delegated = 'Delegated',
  NoProgress = 'NoProgress'
}

export interface CreateAssignmentDto {
  requestId: string;
  assignedToEntityId: string;
  assignedToUserId?: string;
  assignedByUserId: string;
  delegatedFromUserId?: string;
}

export interface DelegateAssignmentDto {
  assignedToEntityId: string;
  assignedToUserId?: string;
  notes?: string;
}

export interface RequestExtensionDto {
  reason: string;
  requestedDueDate: Date;
}

export interface ReportNoProgressDto {
  reason: string;
}

// Helper function for status badge styling
export function getAssignmentStatusClass(status: AssignmentStatus): string {
  switch (status) {
    case AssignmentStatus.Pending:
      return 'bg-slate-150 text-slate-800 dark:bg-navy-500 dark:text-navy-100';
    case AssignmentStatus.InProgress:
      return 'bg-info/10 text-info dark:bg-info/15';
    case AssignmentStatus.Completed:
      return 'bg-success/10 text-success dark:bg-success/15';
    case AssignmentStatus.Overdue:
      return 'bg-error/10 text-error dark:bg-error/15';
    case AssignmentStatus.Delegated:
      return 'bg-warning/10 text-warning dark:bg-warning/15';
    case AssignmentStatus.NoProgress:
      return 'bg-slate-300/50 text-slate-600 dark:bg-navy-450 dark:text-navy-200';
    default:
      return 'bg-slate-150 text-slate-800 dark:bg-navy-500 dark:text-navy-100';
  }
}

// Helper to get assignment status label
export function getAssignmentStatusLabel(status: AssignmentStatus): string {
  switch (status) {
    case AssignmentStatus.Pending:
      return 'Pending';
    case AssignmentStatus.InProgress:
      return 'In Progress';
    case AssignmentStatus.Completed:
      return 'Completed';
    case AssignmentStatus.Overdue:
      return 'Overdue';
    case AssignmentStatus.Delegated:
      return 'Delegated';
    case AssignmentStatus.NoProgress:
      return 'No Progress';
    default:
      return status;
  }
}
