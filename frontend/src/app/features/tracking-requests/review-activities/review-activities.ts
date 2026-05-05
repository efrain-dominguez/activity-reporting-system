import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TrackingRequestsService } from '../tracking-requests.service';
import { ActivitiesService } from '../../activities/activities.service';
import { TrackingRequest } from '../../../shared/models/tracking-request.model';
import { Activity, ReviewStatus, getReviewStatusClass, ReviewActivityDto } from '../../../shared/models/activity.model';
import { ModalService } from '../../../shared/services/modal';

@Component({
  selector: 'app-review-activities',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './review-activities.html',
  styleUrls: ['./review-activities.css']
})
export class ReviewActivities implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private trackingRequestsService = inject(TrackingRequestsService);
  private activitiesService = inject(ActivitiesService);
  private modalService = inject(ModalService);

  trackingRequest?: TrackingRequest;
  activities: Activity[] = [];
  selectedActivity?: Activity;

  reviewComments = '';
  isSubmitting = false;

  // Expose to template
  ReviewStatus = ReviewStatus;
  getReviewStatusClass = getReviewStatusClass;

  ngOnInit(): void {
    const requestId = this.route.snapshot.paramMap.get('id');
    if (requestId) {
      this.loadData(requestId);
    }
  }

  loadData(requestId: string): void {
    // Load tracking request
    this.trackingRequest = this.trackingRequestsService.getTrackingRequestById(requestId);

    if (!this.trackingRequest) {
      this.router.navigate(['/tracking-requests']);
      return;
    }

    // Load activities for this request
    this.activities = this.activitiesService.getActivities()
      .filter(activity => activity.requestId === requestId);

    // Select first activity by default
    if (this.activities.length > 0) {
      this.selectActivity(this.activities[0]);
    }
    // else {
    //    this.modalService.alert({
    //     title: 'No activities found',
    //     message: 'There are no activities associated with this tracking request.',
    //     variant: 'warning'
    //   });
    //    this.router.navigate(['/tracking-requests']);
    //   return;
    // }
  }

  selectActivity(activity: Activity): void {
    this.selectedActivity = activity;
    this.reviewComments = activity.review?.comments || '';
  }

  getActivityStatusClass(activity: Activity): string {
    if (activity.review) {
      return getReviewStatusClass(activity.review.status);
    }
    return activity.isEditable
      ? 'bg-slate-150 text-slate-800 dark:bg-navy-500 dark:text-navy-100'
      : 'bg-warning/10 text-warning dark:bg-warning/15';
  }

  getActivityStatusText(activity: Activity): string {
    if (activity.review) {
      return activity.review.status;
    }
    return activity.isEditable ? 'Draft' : 'Pending Review';
  }

  async onApprove(): Promise<void> {
    if (!this.selectedActivity) return;

    const confirmed = await this.modalService.confirm({
      title: 'Approve Activity',
      message: 'Are you sure you want to approve this activity?',
      confirmText: 'Approve',
      cancelText: 'Cancel',
      variant: 'success'
    });

    if (confirmed) {
      await this.submitReview(ReviewStatus.Approved);
    }
  }

  async onRequestChanges(): Promise<void> {
    if (!this.selectedActivity) return;

    if (!this.reviewComments.trim()) {
      await this.modalService.alert({
        title: 'Comments Required',
        message: 'Please add comments explaining what changes are needed.',
        variant: 'warning'
      });
      return;
    }

    const confirmed = await this.modalService.confirm({
      title: 'Request Changes',
      message: 'This will notify the submitter that changes are needed.',
      confirmText: 'Request Changes',
      cancelText: 'Cancel',
      variant: 'warning'
    });

    if (confirmed) {
      await this.submitReview(ReviewStatus.ChangesRequested);
    }
  }

  async onReject(): Promise<void> {
    if (!this.selectedActivity) return;

    if (!this.reviewComments.trim()) {
      await this.modalService.alert({
        title: 'Comments Required',
        message: 'Please add comments explaining why this activity is being rejected.',
        variant: 'warning'
      });
      return;
    }

    const confirmed = await this.modalService.confirm({
      title: 'Reject Activity',
      message: 'Are you sure you want to reject this activity?',
      confirmText: 'Reject',
      cancelText: 'Cancel',
      variant: 'error'
    });

    if (confirmed) {
      await this.submitReview(ReviewStatus.Rejected);
    }
  }

  private async submitReview(status: ReviewStatus): Promise<void> {
    if (!this.selectedActivity) return;

    this.isSubmitting = true;

    const dto: ReviewActivityDto = {
      status,
      comments: this.reviewComments.trim()
    };

    // TODO: Call API
    console.log('Submit review:', this.selectedActivity.id, dto);

    // For now, update locally
    this.selectedActivity.review = {
      reviewedByUserId: 'currentUser',
      status,
      comments: dto.comments,
      reviewedAt: new Date()
    };

    await this.modalService.alert({
      title: 'Success',
      message: 'Review submitted successfully!',
      variant: 'success'
    });

    this.isSubmitting = false;

    // Refresh to show updated status
    this.loadData(this.trackingRequest!.id);
  }

  downloadFile(file: { fileName: string; blobUrl: string }): void {
    console.log('Download file:', file.fileName);
    // TODO: Implement actual download
    window.open(file.blobUrl, '_blank');
  }

  getFileIcon(fileName: string): string {
    const ext = fileName.split('.').pop()?.toLowerCase();
    switch (ext) {
      case 'pdf':
        return 'M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z';
      case 'xlsx':
      case 'xls':
      case 'csv':
        return 'M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z';
      case 'doc':
      case 'docx':
        return 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z';
      case 'jpg':
      case 'jpeg':
    case 'png':
      case 'gif':
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

  goBack(): void {
    this.router.navigate(['/tracking-requests']);
  }
}
