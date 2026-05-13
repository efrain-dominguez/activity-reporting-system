import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AssignmentsService } from '../../shared/services/assignments.service';
import { TrackingRequestsService } from '../tracking-requests/tracking-requests.service';
import { ActivitiesService } from '../activities/activities.service';
import { RequestAssignment, AssignmentStatus, getAssignmentStatusClass, getAssignmentStatusLabel } from '../../shared/models/assignment.model';
import { TrackingRequest } from '../../shared/models/tracking-request.model';
import { Entity } from '../../shared/models/entity.model';
import { ModalService } from '../../shared/services/modal';
import { Activity } from '../../shared/models/activity.model';
import { getReviewStatusClass } from '../../shared/models/activity.model';

@Component({
  selector: 'app-assignments',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './assignments.html',
  styleUrls: ['./assignments.css']
})
export class Assignments implements OnInit {
  private assignmentsService = inject(AssignmentsService);
  private trackingRequestsService = inject(TrackingRequestsService);
  private activitiesService = inject(ActivitiesService);
  private modalService = inject(ModalService);
  private cdr = inject(ChangeDetectorRef);
  showSubmitActivityModal = false;

  // Current tab
  activeTab: 'myAssignments' | 'delegatedByMe' | 'teamOverview' = 'myAssignments';

  // Data
  myAssignments: RequestAssignment[] = [];
  delegatedAssignments: RequestAssignment[] = [];
  entityAssignments: RequestAssignment[] = [];
  showViewActivitiesModal = false;
  viewingActivities: Activity[] = [];

  // Stats
  stats = {
    total: 0,
    pending: 0,
    inProgress: 0,
    completed: 0,
    overdue: 0,
    delegated: 0
  };

  // Modal states
  showDelegateModal = false;
  showNoProgressModal = false;
  showExtensionModal = false;
  selectedAssignment?: RequestAssignment;

  // Form data
  delegateToEntityId = '';
  delegateToUserId = '';
  delegateNotes = '';
  noProgressReason = '';
  extensionReason = '';
  extensionRequestedDate?: Date;

  // Submit Activity Modal - Form Data
  activityDate: string = '';
  activityDescription: string = '';
  selectedFiles: File[] = [];


// Mock current user and entity (replace with auth service)
  currentUserId = 'user1';
  currentEntityId = 'entity1';
  currentUserRole: 'Director' | 'AssistantDirectorate' | 'Team' = 'Director';

  // Mock entities
  entities: Entity[] = [
    { id: 'entity1', name: 'Finance Department', code: 'FIN', description: 'Financial operations and accounting', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity2', name: 'HR Department', code: 'HR', description: 'Human resources and personnel', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity3', name: 'IT Department', code: 'IT', description: 'Information technology and systems', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity4', name: 'Marketing Department', code: 'MKT', description: 'Marketing and communications', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity5', name: 'Operations Department', code: 'OPS', description: 'Operations and logistics', isActive: true, createdAt: new Date(), updatedAt: new Date() }
  ];

  // Expose to template
  AssignmentStatus = AssignmentStatus;
  getAssignmentStatusClass = getAssignmentStatusClass;
  getAssignmentStatusLabel = getAssignmentStatusLabel;
  getReviewStatusClass = getReviewStatusClass;
  Math = Math;

  ngOnInit(): void {
    this.loadStats();
    this.loadTabData();
  }

  loadStats(): void {
    this.stats = this.assignmentsService.getEntityStats(this.currentEntityId);
  }

// Load data for current active tab
  loadTabData(): void {
    switch (this.activeTab) {
      case 'myAssignments':
        this.myAssignments = this.assignmentsService.getMyAssignments(this.currentUserId);
        console.log('My Assignments loaded:', this.myAssignments);
        break;
      case 'delegatedByMe':
        this.delegatedAssignments = this.assignmentsService.getDelegatedByMe(this.currentUserId);
        console.log('Delegated by Me loaded:', this.delegatedAssignments);
        break;
      case 'teamOverview':
        this.entityAssignments = this.assignmentsService.getEntityAssignments(this.currentEntityId);
        console.log('Entity Assignments loaded:', this.entityAssignments);
        break;
    }

    this.cdr.detectChanges();
  }

  loadAllTabData(): void {
    this.myAssignments = this.assignmentsService.getMyAssignments(this.currentUserId);
    this.delegatedAssignments = this.assignmentsService.getDelegatedByMe(this.currentUserId);
    this.entityAssignments = this.assignmentsService.getEntityAssignments(this.currentEntityId);
    console.log('All tabs data reloaded');
    this.cdr.detectChanges();
  }

  setActiveTab(tab: 'myAssignments' | 'delegatedByMe' | 'teamOverview'): void {
    this.activeTab = tab;
    this.loadTabData();
  }

  getTrackingRequest(requestId: string): TrackingRequest | undefined {
    return this.trackingRequestsService.getTrackingRequestById(requestId);
  }

  getEntity(entityId: string): Entity | undefined {
    return this.entities.find(e => e.id === entityId);
  }

  getActivityCount(requestId: string): number {
    return this.activitiesService.getActivitiesByRequestId(requestId).length;
  }

  getDaysRemaining(assignment: RequestAssignment): number {
    const request = this.getTrackingRequest(assignment.requestId);
    if (!request) return 0;

    const dueDate = assignment.newDueDate || request.dueDate;
    const today = new Date();
    const diffTime = dueDate.getTime() - today.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  isOverdue(assignment: RequestAssignment): boolean {
    return this.getDaysRemaining(assignment) < 0;
  }

  // Delegate assignment
  onDelegate(assignment: RequestAssignment): void {
    this.selectedAssignment = assignment;
    this.delegateToEntityId = '';
    this.delegateToUserId = '';
    this.delegateNotes = '';
    this.showDelegateModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

async submitDelegation(): Promise<void> {
  // Validation
  if (!this.delegateToEntityId) {
    await this.modalService.alert({
      title: 'Missing Information',
      message: 'Please select an entity to delegate to.',
      variant: 'warning'
    });
    return;
  }

  if (!this.selectedAssignment) {
    return;
  }

  // Get entity name for confirmation message
  const entityName = this.getEntity(this.delegateToEntityId)?.name || 'selected entity';

  // Ask for confirmation (modal stays open)
  const confirmed = await this.modalService.confirm({
    title: 'Confirm Delegation',
    message: `Are you sure you want to delegate this assignment to ${entityName}?`,
    confirmText: 'Delegate',
    cancelText: 'Cancel',
    variant: 'info'
  });

  if (!confirmed) {
    return; // User cancelled - stay in modal
  }

  // Perform delegation
  const newAssignment = this.assignmentsService.delegateAssignment(
    this.selectedAssignment.id,
    this.delegateToEntityId,
    undefined,
    this.delegateNotes
  );

  if (!newAssignment) {
    await this.modalService.alert({
      title: 'Delegation Failed',
      message: 'Failed to delegate assignment. Please try again.',
      variant: 'error'
    });
    return;
  }

  // Close the delegate modal NOW (after confirmation)
  this.closeDelegateModal();

  // Show success
  await this.modalService.alert({
    title: 'Delegation Successful',
    message: `Assignment has been delegated to ${entityName}.`,
    variant: 'success'
  });

  // Reset form
  this.delegateToEntityId = '';
  this.delegateNotes = '';

  // Reload all data
  this.loadStats();
  this.loadAllTabData();

  this.cdr.detectChanges();
}
  closeDelegateModal(): void {
    this.showDelegateModal = false;
    this.selectedAssignment = undefined;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  // Report no progress
  onReportNoProgress(assignment: RequestAssignment): void {
    this.selectedAssignment = assignment;
    this.noProgressReason = '';
    this.showNoProgressModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

async submitNoProgress(): Promise<void> {
  // Validation
  if (!this.noProgressReason.trim()) {
    await this.modalService.alert({
      title: 'Missing Information',
      message: 'Please provide a reason for no progress.',
      variant: 'warning'
    });
    return;
  }

  if (!this.selectedAssignment) {
    return;
  }

  // Confirm this serious action (modal stays open)
  const confirmed = await this.modalService.confirm({
    title: 'Confirm No Progress Report',
    message: 'This will notify the PMO that no progress can be made and mark the assignment accordingly. Are you sure?',
    confirmText: 'Report No Progress',
    cancelText: 'Cancel',
    variant: 'warning'
  });

  if (!confirmed) {
    return; // User cancelled - stay in modal
  }

  // Submit no progress report
  const success = this.assignmentsService.reportNoProgress(
    this.selectedAssignment.id,
    this.noProgressReason
  );

  if (!success) {
    await this.modalService.alert({
      title: 'Report Failed',
      message: 'Failed to submit no progress report. Please try again.',
      variant: 'error'
    });
    return;
  }

  // Close the no progress modal NOW (after confirmation)
  this.closeNoProgressModal();

  await this.modalService.alert({
    title: 'Report Submitted',
    message: 'No progress report has been submitted to PMO.',
    variant: 'success'
  });

  // Reset form
  this.noProgressReason = '';

  // Reload all data
  this.loadStats();
  this.loadAllTabData();

  this.cdr.detectChanges();
}

  closeNoProgressModal(): void {
    this.showNoProgressModal = false;
    this.selectedAssignment = undefined;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  // Request extension
  onRequestExtension(assignment: RequestAssignment): void {
    this.selectedAssignment = assignment;
    this.extensionReason = '';
    this.extensionRequestedDate = undefined;
    this.showExtensionModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }


  async submitExtensionRequest(): Promise<void> {
  // Validation
  if (!this.extensionRequestedDate || !this.extensionReason.trim()) {
    await this.modalService.alert({
      title: 'Missing Information',
      message: 'Please provide both a new due date and a reason for the extension.',
      variant: 'warning'
    });
    return;
  }

  if (!this.selectedAssignment) {
    return;
  }

  // Validate new date is after current due date
  const currentDueDate = this.getTrackingRequest(this.selectedAssignment.requestId)?.dueDate;
  if (currentDueDate && new Date(this.extensionRequestedDate) <= new Date(currentDueDate)) {
    await this.modalService.alert({
      title: 'Invalid Date',
      message: 'The new due date must be after the current due date.',
      variant: 'warning'
    });
    return;
  }

  // Confirm action (modal stays open)
  const confirmed = await this.modalService.confirm({
    title: 'Request Extension',
    message: 'This extension request will be sent to PMO for approval. Continue?',
    confirmText: 'Submit Request',
    cancelText: 'Cancel',
    variant: 'info'
  });

  if (!confirmed) {
    return; // User cancelled - stay in modal
  }

  // Submit extension request
  const success = this.assignmentsService.requestExtension(
    this.selectedAssignment.id,
    String(this.extensionRequestedDate),
    this.extensionReason
  );

  if (!success) {
    await this.modalService.alert({
      title: 'Request Failed',
      message: 'Failed to submit extension request. Please try again.',
      variant: 'error'
    });
    return;
  }

  // Close the extension modal NOW (after confirmation)
  this.closeExtensionModal();

  await this.modalService.alert({
    title: 'Request Submitted',
    message: 'Extension request has been submitted for PMO approval.',
    variant: 'success'
  });

  // Reset form
  this.extensionRequestedDate = undefined;
  this.extensionReason = '';

  // Reload all data
  this.loadStats();
  this.loadAllTabData();

  this.cdr.detectChanges();
}


  closeExtensionModal(): void {
    this.showExtensionModal = false;
    this.selectedAssignment = undefined;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  onSubmitActivity(assignment: RequestAssignment): void {
    this.selectedAssignment = assignment;
    this.showSubmitActivityModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  closeSubmitActivityModal(): void {
    this.showSubmitActivityModal = false;
    this.selectedAssignment = undefined;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }


  onViewActivities(assignment: RequestAssignment): void {
    this.selectedAssignment = assignment;

    // Get all activities for this request
    const allActivities = this.activitiesService.getActivitiesByRequestId(assignment.requestId);

    // FIRST: Filter to only activities for THIS SPECIFIC ASSIGNMENT
    const assignmentActivities = allActivities.filter(activity =>
      activity.assignmentId === assignment.id
    );

    // THEN: Filter based on user role
    if (this.currentUserRole === 'Director') {
      // Directors can see ALL activities for this specific assignment
      this.viewingActivities = assignmentActivities;
    } else if (this.currentUserRole === 'AssistantDirectorate') {
      // Assistant Directorate can only see:
      // 1. Their own activities
      // 2. Activities from assignments they delegated
      this.viewingActivities = assignmentActivities.filter(activity =>
        activity.submittedByUserId === this.currentUserId || // Their own activities
        assignment.delegatedFromUserId === this.currentUserId // Delegated by them
      );
    } else {
      // Team members can only see their own activities
      this.viewingActivities = assignmentActivities.filter(activity =>
        activity.submittedByUserId === this.currentUserId
      );
    }

    this.showViewActivitiesModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  canViewActivities(assignment: RequestAssignment): boolean {
   if (this.currentUserRole === 'Director') {
      // Directors can always view activities for their entity
      return assignment.assignedToEntityId === this.currentEntityId;
    } else if (this.currentUserRole === 'AssistantDirectorate') {
      // Assistant Directorate can view if:
      // 1. Assignment is theirs
      // 2. They delegated it
      return assignment.assignedToUserId === this.currentUserId ||
            assignment.delegatedFromUserId === this.currentUserId;
    } else {
      // Team members can only view their own assignments
      return assignment.assignedToUserId === this.currentUserId;
    }
   }

  closeViewActivitiesModal(): void {
    this.showViewActivitiesModal = false;
    this.selectedAssignment = undefined;
    this.viewingActivities = [];
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  getFileIcon(fileName: string): string {
    const ext = fileName.split('.').pop()?.toLowerCase();
    switch (ext) {
      case 'pdf':
        return 'M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z';
      case 'xlsx':
      case 'xls':
        return 'M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z';
      case 'docx':
      case 'doc':
        return 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z';
      case 'jpg':
      case 'png':
        return 'M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z';
      default:
        return 'M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z';
    }
  }

  formatFileSize(bytes?: number): string {
    if (!bytes) return 'Unknown size';
    const kb = bytes / 1024;
    if (kb < 1024) return `${kb.toFixed(1)} KB`;
    const mb = kb / 1024;
    return `${mb.toFixed(1)} MB`;
  }

  downloadFile(file: { fileName: string; blobUrl: string }): void {
    window.open(file.blobUrl, '_blank');
  }


  onFileSelected(event: any): void {
    const files: FileList = event.target.files;

    for (let i = 0; i < files.length; i++) {
      const file = files[i];

      // Check file size (4MB max)
      if (file.size > 4 * 1024 * 1024) {
        this.modalService.alert({
          title: 'File Too Large',
          message: `${file.name} exceeds the maximum size of 4MB.`,
          variant: 'error'
        });
        continue;
      }

      this.selectedFiles.push(file);
    }

    event.target.value = '';
  }

  // Remove a selected file
  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);
  }

  async submitActivity(): Promise<void> {
    // Validation
    if (!this.selectedAssignment || !this.activityDate || !this.activityDescription.trim()) {
      await this.modalService.alert({
        title: 'Missing Information',
        message: 'Please fill in all required fields.',
        variant: 'warning'
      });
      return;
    }

    // Create new activity object
    const newActivity: Activity = {
      id: 'act' + Date.now(),
      requestId: this.selectedAssignment.requestId,
      assignmentId: this.selectedAssignment.id,
      activityDate: new Date(this.activityDate),
      description: this.activityDescription.trim(),
      submittedByUserId: this.currentUserId,
      submittedAt: new Date(),
      files: this.selectedFiles.map(file => ({
        fileName: file.name,
        blobUrl: URL.createObjectURL(file),
        fileSizeBytes: file.size,
        fileType: file.type,
        uploadedAt: new Date(),
        fileId: 'file' + Date.now() + Math.random().toString(16).slice(2) // Mock file ID
      })),
      isEditable: false,
      review: undefined,
      createdAt: new Date(),
      updatedAt: new Date()
    };

    // Save activity through service
    this.activitiesService.addActivity(newActivity);

    // Show success message
    await this.modalService.alert({
      title: 'Success',
      message: 'Activity submitted successfully!',
      variant: 'success'
    });

    // Reset form
    this.activityDate = '';
    this.activityDescription = '';
    this.selectedFiles = [];

    // Close modal
    this.closeSubmitActivityModal();

    // Reload tab data
    this.loadAllTabData();

    this.cdr.detectChanges();
  }
}
