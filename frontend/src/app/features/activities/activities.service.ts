import { Injectable } from '@angular/core';
import { Activity, CreateActivityDto, UpdateActivityDto, ActivityFile, ReviewActivityDto, ReviewStatus } from '../../shared/models/activity.model';

@Injectable({
  providedIn: 'root'
})
export class ActivitiesService {

  // Mock data
 private mockActivities: Activity[] = [
  {
    id: 'act1',
    assignmentId: 'assign1',
    requestId: 'req1',
    description: 'Completed financial compliance review for Finance Department. All documents verified and approved.',
    activityDate: new Date('2024-01-15'),
    submittedByUserId: 'user1',
    files: [
      {
        fileName: 'compliance_report.pdf',
        blobUrl: '/files/compliance_report.pdf',
        uploadedAt: new Date('2024-01-15'),
        fileSizeBytes: 2457600, // ~2.4 MB
        fileType: 'application/pdf',
        fileId: ''
      },
      {
        fileName: 'verification_docs.xlsx',
        blobUrl: '/files/verification_docs.xlsx',
        uploadedAt: new Date('2024-01-15'),
        fileSizeBytes: 1048576, // 1 MB
        fileType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        fileId: ''
      }
    ],
    isEditable: false,
    createdAt: new Date('2024-01-15'),
    updatedAt: new Date('2024-01-15'),
    submittedAt: new Date('2024-01-15')
  },
  {
    id: 'act2',
    assignmentId: 'assign2',
    requestId: 'req1',
    description: 'Conducted safety training session for HR department. 25 employees completed certification.',
    activityDate: new Date('2024-02-10'),
    submittedByUserId: 'user1',
    files: [
      {
        fileName: 'training_roster.pdf',
        blobUrl: '/files/training_roster.pdf',
        uploadedAt: new Date('2024-02-10'),
        fileSizeBytes: 512000, // 500 KB
        fileType: 'application/pdf',
        fileId: ''
      }
    ],
    isEditable: false,
    createdAt: new Date('2024-02-10'),
    updatedAt: new Date('2024-02-10'),
    submittedAt: new Date('2024-02-10')
  },
  {
    id: 'act3',
    assignmentId: 'assign3',
    requestId: 'req1',
    description: 'IT security audit completed for all systems. No critical vulnerabilities found.',
    activityDate: new Date('2024-01-20'),
    submittedByUserId: 'user1',
    files: [
      {
        fileName: 'security_audit.pdf',
        blobUrl: '/files/security_audit.pdf',
        uploadedAt: new Date('2024-01-20'),
        fileSizeBytes: 3145728,  // 3 MB
        fileType: 'application/pdf',
        fileId: ''
      },
      {
        fileName: 'scan_results.xlsx',
        blobUrl: '/files/scan_results.xlsx',
        uploadedAt: new Date('2024-01-20'),
        fileSizeBytes: 204800,  // 200 KB
        fileType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        fileId: ''
      }
    ],
    isEditable: false,
    createdAt: new Date('2024-01-20'),
    updatedAt: new Date('2024-01-22'),
    submittedAt: new Date('2024-01-20')
  },
  {
    id: 'act4',
    assignmentId: 'assign4',
    requestId: 'req2',
    description: 'Monthly employee training completion tracking for all departments.',
    activityDate: new Date('2024-02-15'),
    submittedByUserId: 'user2',
    files: [],
    isEditable: false,
    createdAt: new Date('2024-02-15'),
    updatedAt: new Date('2024-02-15'),
    submittedAt: new Date('2024-02-15')
  }
];

  // Get all activities (returns array directly for mock data)
  getActivities(): Activity[] {
    return this.mockActivities;
  }

  // Get single activity by ID
  getActivityById(id: string): Activity | undefined {
    return this.mockActivities.find(act => act.id === id);
  }

  // Create new activity
  createActivity(dto: CreateActivityDto): Activity {
    const newActivity: Activity = {
      id: 'act' + (this.mockActivities.length + 1),
      ...dto,
      submittedByUserId: 'currentUser',
      files: [],
      isEditable: true,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    this.mockActivities.push(newActivity);
    return newActivity;
  }

  // Update activity
  updateActivity(id: string, dto: UpdateActivityDto): Activity | undefined {
    const index = this.mockActivities.findIndex(act => act.id === id);
    if (index !== -1 && this.mockActivities[index].isEditable) {
      this.mockActivities[index] = {
        ...this.mockActivities[index],
        ...dto,
        updatedAt: new Date()
      };
      return this.mockActivities[index];
    }
    return undefined;
  }

  // Submit activity (makes it non-editable)
  submitActivity(id: string): Activity | undefined {
    const activity = this.mockActivities.find(act => act.id === id);
    if (activity && activity.isEditable) {
      activity.isEditable = false;
      activity.submittedAt = new Date();
      activity.updatedAt = new Date();
      return activity;
    }
    return undefined;
  }

  // Delete activity
  deleteActivity(id: string): boolean {
    const index = this.mockActivities.findIndex(act => act.id === id);
    if (index !== -1) {
      this.mockActivities.splice(index, 1);
      return true;
    }
    return false;
  }

  // Upload file to activity
  uploadFile(activityId: string, file: File): ActivityFile | undefined {
    const activity = this.mockActivities.find(act => act.id === activityId);
    if (activity && activity.isEditable) {
      const newFile: ActivityFile = {
        fileName: file.name,
        blobUrl: '/files/' + file.name,
        uploadedAt: new Date(),
        fileId: 'file' + (activity.files.length + 1),
        fileType: file.type,
        fileSizeBytes: file.size
      };
      activity.files.push(newFile);
      activity.updatedAt = new Date();
      return newFile;
    }
    return undefined;
  }

  // Delete file from activity
  deleteFile(activityId: string, fileName: string): boolean {
    const activity = this.mockActivities.find(act => act.id === activityId);
    if (activity && activity.isEditable) {
      const index = activity.files.findIndex(f => f.fileName === fileName);
      if (index !== -1) {
        activity.files.splice(index, 1);
        activity.updatedAt = new Date();
        return true;
      }
    }
    return false;
  }

  reviewActivity(activityId: string, dto: ReviewActivityDto): Activity | undefined {
    const activity = this.mockActivities.find(act => act.id === activityId);
    if (activity && !activity.isEditable) {
      activity.review = {
        reviewedByUserId: 'currentUser', // TODO: Get from auth service
        status: dto.status,
        comments: dto.comments,
        reviewedAt: new Date()
      };
      activity.updatedAt = new Date();
      return activity;
    }
    return undefined;
  }

  // Get activities by request ID
  getActivitiesByRequestId(requestId: string): Activity[] {
    return this.mockActivities.filter(act => act.requestId === requestId);
  }

  // Get pending review count for a request
  getPendingReviewCount(requestId: string): number {
    return this.mockActivities.filter(act =>
      act.requestId === requestId &&
      !act.isEditable &&
      (!act.review || act.review.status === ReviewStatus.Pending)
    ).length;
  }
}
