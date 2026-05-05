export interface Activity {
  id: string;
  assignmentId: string;
  requestId: string;
  description: string;
  activityDate: Date;
  submittedByUserId: string;
  files: ActivityFile[];
  isEditable: boolean;
  createdAt: Date;
  updatedAt?: Date;
  submittedAt?: Date;
  review?: ActivityReview;
}

export interface ActivityFile {
  fileId: string;
  fileName: string;
  fileType: string;
  fileSizeBytes: number;
  blobUrl: string;
  uploadedAt: Date;
}

export interface ActivityReview {
  reviewedByUserId: string;
  status: ReviewStatus;
  comments: string;
  reviewedAt: Date;
}

export enum ReviewStatus {
  Pending = 'Pending Review',
  Approved = 'Approved',
  Rejected = 'Rejected',
  ChangesRequested = 'Changes Requested'
}

export interface ReviewActivityDto {
  status: ReviewStatus;
  comments: string;
}

export function getReviewStatusClass(status: ReviewStatus): string {
  switch (status) {
    case ReviewStatus.Pending:
      return 'bg-warning/10 text-warning dark:bg-warning/15';
    case ReviewStatus.Approved:
      return 'bg-success/10 text-success dark:bg-success/15';
    case ReviewStatus.Rejected:
      return 'bg-error/10 text-error dark:bg-error/15';
    case ReviewStatus.ChangesRequested:
      return 'bg-info/10 text-info dark:bg-info/15';
    default:
      return 'bg-slate-150 text-slate-800 dark:bg-navy-500 dark:text-navy-100';
  }
}

// DTO for creating new activities
export interface CreateActivityDto {
  assignmentId: string;
  requestId: string;
  description: string;
  activityDate: Date;
  files?: File[];
}

// DTO for updating activities
export interface UpdateActivityDto {
  description?: string;
  activityDate?: Date;
  filesToAdd?: File[];
  fileIdsToRemove?: string[];
}

// Activity status - more comprehensive
export enum ActivityStatus {
  Draft = 'Draft',
  Submitted = 'Submitted',
  InReview = 'In Review' // Added this
}

// Helper function to get activity status
// Note: We'll need review data to determine if it's "In Review"
// For now, we just check isEditable and submittedAt
export function getActivityStatus(activity: Activity, isUnderReview?: boolean): ActivityStatus {
  if (activity.isEditable) {
    return ActivityStatus.Draft;
  }

  // If not editable and under review
  if (isUnderReview) {
    return ActivityStatus.InReview;
  }

  // If not editable and has been submitted
  return ActivityStatus.Submitted;
}
