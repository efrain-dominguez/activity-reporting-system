import { Component, OnInit, AfterViewInit, inject, ChangeDetectorRef, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivitiesService } from './activities.service';
import { Activity, ActivityStatus, getActivityStatus, CreateActivityDto } from '../../shared/models/activity.model';
import { ActivityForm } from './activity-form/activity-form';
import { ModalService } from '../../shared/services/modal';
import { DataTable } from '../../shared/components/data-table/data-table';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../shared/components/data-table/data-table.models';

@Component({
  selector: 'app-activities',
  standalone: true,
  imports: [CommonModule, RouterModule, ActivityForm, DataTable],
  templateUrl: './activities.html',
  styleUrls: ['./activities.css']
})
export class Activities implements OnInit, AfterViewInit {
  private activitiesService = inject(ActivitiesService);
  private cdr = inject(ChangeDetectorRef);
  private modalService = inject(ModalService);

  // Data
  activities: Activity[] = [];

  // Table configuration
  columns: DataTableColumn<Activity>[] = [];
  actions: DataTableAction<Activity>[] = [];
  tableConfig: DataTableConfig = {
    searchPlaceholder: 'Search activities...',
    defaultPageSize: 10,
    defaultSortColumn: 'activityDate',
    defaultSortDirection: 'desc'
  };

  // Templates
  @ViewChild('assignmentCell', { static: true }) assignmentCell!: TemplateRef<any>;
  @ViewChild('requestCell', { static: true }) requestCell!: TemplateRef<any>;
  @ViewChild('dateCell', { static: true }) dateCell!: TemplateRef<any>;
  @ViewChild('statusCell', { static: true }) statusCell!: TemplateRef<any>;
  @ViewChild('filesCell', { static: true }) filesCell!: TemplateRef<any>;

  // Modal state
  showCreateModal = false;
  editingActivity?: Activity;

  // Expose enums to template
  ActivityStatus = ActivityStatus;
  getActivityStatus = getActivityStatus;

  ngOnInit(): void {
    this.setupTable();
    this.loadActivities();
  }

  ngAfterViewInit(): void {
    // Link templates to columns
    this.columns[0].cellTemplate = this.assignmentCell;
    this.columns[1].cellTemplate = this.requestCell;
    this.columns[2].cellTemplate = this.dateCell;
    this.columns[3].cellTemplate = this.statusCell;
    this.columns[4].cellTemplate = this.filesCell;

    this.cdr.detectChanges();
  }

  setupTable(): void {
    // Define columns
    this.columns = [
      { key: 'assignmentId', label: 'Assignment', sortable: true },
      { key: 'requestId', label: 'Request', sortable: true },
      { key: 'activityDate', label: 'Date', sortable: true },
      { key: 'status', label: 'Status', sortable: true, valueGetter: (row) => getActivityStatus(row) },
      { key: 'files', label: 'Files', sortable: false, valueGetter: (row) => row.files.length }
    ];

    // Define actions
    this.actions = [
      {
        label: 'View',
        icon: 'M15 12a3 3 0 11-6 0 3 3 0 016 0z M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z',
        variant: 'default',
        handler: (row) => this.onViewActivity(row)
      },
      {
        label: 'Edit',
        icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
        variant: 'default',
        visible: (row) => row.isEditable,
        handler: (row) => this.onEditActivity(row)
      },
      {
        label: 'Delete',
        icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
        variant: 'error',
        handler: (row) => this.onDeleteActivity(row)
      }
    ];
  }

  loadActivities(): void {
    this.activities = this.activitiesService.getActivities();
  }

  // CRUD operations
  onNewActivity(): void {
    this.editingActivity = undefined;
    this.showCreateModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  onViewActivity(activity: Activity): void {
    console.log('View activity:', activity);
  }

  async onEditActivity(activity: Activity): Promise<void> {
    if (!activity.isEditable) {
      await this.modalService.alert({
        title: 'Cannot Edit',
        message: 'This activity has been submitted and cannot be edited.',
        variant: 'warning'
      });
      return;
    }
    this.editingActivity = activity;
    this.showCreateModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  async onDeleteActivity(activity: Activity): Promise<void> {
    const confirmed = await this.modalService.confirm({
      title: 'Delete Activity',
      message: 'Are you sure you want to delete this activity? This action cannot be undone.',
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'error'
    });

    if (confirmed) {
      console.log('Delete activity:', activity.id);
    }
  }

  async onSaveActivity(dto: CreateActivityDto): Promise<void> {
    console.log('Save activity:', dto);

    await this.modalService.alert({
      title: 'Success',
      message: 'Activity saved successfully!',
      variant: 'success'
    });

    this.closeModal();
    this.loadActivities();
  }

  closeModal(): void {
    this.showCreateModal = false;
    this.editingActivity = undefined;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  getStatusBadgeClass(status: ActivityStatus): string {
    switch (status) {
      case ActivityStatus.Draft:
        return 'bg-slate-150 text-slate-800 dark:bg-navy-500 dark:text-navy-100';
      case ActivityStatus.Submitted:
        return 'bg-info/10 text-info dark:bg-info/15';
      case ActivityStatus.InReview:
        return 'bg-warning/10 text-warning dark:bg-warning/15';
      default:
        return '';
    }
  }
}
