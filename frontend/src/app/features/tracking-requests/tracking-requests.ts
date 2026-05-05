import { Component, OnInit, AfterViewInit, inject, ChangeDetectorRef, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TrackingRequestsService } from './tracking-requests.service';
import { TrackingRequest, RequestStatus, getRequestStatusClass, isRequestOverdue, CreateTrackingRequestDto } from '../../shared/models/tracking-request.model';
import { TrackingRequestForm } from './tracking-request-form/tracking-request-form';
import { ModalService } from '../../shared/services/modal';
import { DataTable } from '../../shared/components/data-table/data-table';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../shared/components/data-table/data-table.models';
import { Entity } from '../../shared/models/entity.model';
import { TooltipDirective } from '../../shared/directives/tooltip';
import { ActivitiesService } from '../activities/activities.service';

@Component({
  selector: 'app-tracking-requests',
  standalone: true,
  imports: [CommonModule, RouterModule, TrackingRequestForm, DataTable, TooltipDirective],
  templateUrl: './tracking-requests.html',
  styleUrls: ['./tracking-requests.css']
})
export class TrackingRequests implements OnInit, AfterViewInit {
  private trackingRequestsService = inject(TrackingRequestsService);
  private activitiesService = inject(ActivitiesService);
  private cdr = inject(ChangeDetectorRef);
  private modalService = inject(ModalService);

  // Data
  requests: TrackingRequest[] = [];

    // Mock entities (same as in form)
  entities: Entity[] = [
    { id: 'entity1', name: 'Finance Department', code: 'FIN', description: 'Financial operations and accounting', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity2', name: 'HR Department', code: 'HR', description: 'Human resources and personnel', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity3', name: 'IT Department', code: 'IT', description: 'Information technology and systems', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity4', name: 'Marketing Department', code: 'MKT', description: 'Marketing and communications', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity5', name: 'Operations Department', code: 'OPS', description: 'Operations and logistics', isActive: true, createdAt: new Date(), updatedAt: new Date() }
  ];


  // Table configuration
  columns: DataTableColumn<TrackingRequest>[] = [];
  actions: DataTableAction<TrackingRequest>[] = [];
  tableConfig: DataTableConfig = {
    searchPlaceholder: 'Search requests...',
    defaultPageSize: 15,
    defaultSortColumn: 'dueDate',
    defaultSortDirection: 'asc'
  };

  // Templates
  @ViewChild('titleCell', { static: true }) titleCell!: TemplateRef<any>;
  @ViewChild('goalTypeCell', { static: true }) goalTypeCell!: TemplateRef<any>;
  @ViewChild('statusCell', { static: true }) statusCell!: TemplateRef<any>;
  @ViewChild('dueDateCell', { static: true }) dueDateCell!: TemplateRef<any>;
  @ViewChild('entitiesCell', { static: true }) entitiesCell!: TemplateRef<any>;
  @ViewChild('activitiesCell', { static: true }) activitiesCell!: TemplateRef<any>;
  @ViewChild('createdAtCell', { static: true }) createdAtCell!: TemplateRef<any>;
  @ViewChild('recurringCell', { static: true }) recurringCell!: TemplateRef<any>;

  // Modal state
  showCreateModal = false;
  showViewModal = false;
  editingRequest?: TrackingRequest;
  viewingRequest?: TrackingRequest;

  // Expose enums to template
  RequestStatus = RequestStatus;
  getRequestStatusClass = getRequestStatusClass;
  isRequestOverdue = isRequestOverdue;

  ngOnInit(): void {
    this.setupTable();
    this.loadRequests();
  }

  ngAfterViewInit(): void {
    // Link templates to columns
    this.columns[0].cellTemplate = this.titleCell;
    this.columns[1].cellTemplate = this.goalTypeCell;
    this.columns[2].cellTemplate = this.statusCell;
    this.columns[3].cellTemplate = this.dueDateCell;
    // this.columns[4].cellTemplate = this.entitiesCell;
    this.columns[4].cellTemplate = this.activitiesCell;
    // this.columns[6].cellTemplate = this.createdAtCell;
    // this.columns[7].cellTemplate = this.recurringCell;

    this.cdr.detectChanges();
  }

  setupTable(): void {
    // Define columns (templates will be set in ngAfterViewInit)
    this.columns = [
      { key: 'title', label: 'Title', sortable: true, width: '25%' },
      { key: 'goalType', label: 'Goal Type', sortable: true, width: '12%' },
      { key: 'status', label: 'Status', sortable: true, width: '12%' },
      { key: 'dueDate', label: 'Due Date', sortable: true, width: '15%' },
      // { key: 'entities', label: 'Entities', sortable: true, width: '10%', valueGetter: (row) => row.targetEntityIds.length },
      { key: 'activities', label: 'Activities', sortable: true, width: '12%', valueGetter: (row) => this.getActivityCount(row.id) },
      // { key: 'createdAt', label: 'Created At', sortable: true },
      // { key: 'recurring', label: 'Recurring', sortable: false, width: '10%' }
    ];

    // Define actions
    this.actions = [
      {
        label: 'View',
        icon: 'M15 12a3 3 0 11-6 0 3 3 0 016 0z M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z',
        variant: 'default',
        handler: (row) => this.onViewRequest(row)
      },
      {
        label: 'Edit',
        icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
        variant: 'default',
        handler: (row) => this.onEditRequest(row)
      },
      {
        label: 'Submit',
        icon: 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z',
        variant: 'success',
        visible: (row) => row.status === RequestStatus.Draft,
        handler: (row) => this.onSubmitRequest(row)
      },
      {
        label: 'Delete',
        icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
        variant: 'error',
        handler: (row) => this.onDeleteRequest(row)
      }
    ];
  }

  loadRequests(): void {
    // Direct assignment for mock data (no Observable)
    this.requests = this.trackingRequestsService.getTrackingRequests();
  }

  getActivityCount(requestId: string): number {
    return this.activitiesService.getActivitiesByRequestId(requestId).length;
  }

  // ADD THIS METHOD
  getPendingReviewCount(requestId: string): number {
    return this.activitiesService.getPendingReviewCount(requestId);
  }

  getDaysRemaining(request: TrackingRequest): number {
    const today = new Date();
    const dueDate = new Date(request.dueDate);
    const diffTime = dueDate.getTime() - today.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  getDaysOverdue(request: TrackingRequest): number {
    return Math.abs(this.getDaysRemaining(request));
  }

  getEntityDetails(entityIds: string[]): Entity[] {
    return entityIds
      .map(id => this.entities.find(e => e.id === id))
      .filter(e => e !== undefined) as Entity[];
  }

  // CRUD operations
  onNewRequest(): void {
    this.editingRequest = undefined;
    this.showCreateModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  onViewRequest(request: TrackingRequest): void {
    this.viewingRequest = request;
    this.showViewModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  closeViewModal(): void {
    this.showViewModal = false;
    this.viewingRequest = undefined;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  onEditRequest(request: TrackingRequest): void {
    this.editingRequest = request;
    this.showCreateModal = true;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }

  async onDeleteRequest(request: TrackingRequest): Promise<void> {
    const confirmed = await this.modalService.confirm({
      title: 'Delete Tracking Request',
      message: `Are you sure you want to delete "${request.title}"? This action cannot be undone.`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'error'
    });

    if (confirmed) {
      const success = this.trackingRequestsService.deleteTrackingRequest(request.id);
      if (success) {
        await this.modalService.alert({
          title: 'Success',
          message: 'Tracking request deleted successfully!',
          variant: 'success'
        });
        this.loadRequests();
      }
    }
  }

  async onSubmitRequest(request: TrackingRequest): Promise<void> {
    const confirmed = await this.modalService.confirm({
      title: 'Submit Tracking Request',
      message: `Are you sure you want to submit "${request.title}"? Once submitted, it cannot be edited.`,
      confirmText: 'Submit',
      cancelText: 'Cancel',
      variant: 'warning'
    });

    if (confirmed) {
      const updated = this.trackingRequestsService.submitTrackingRequest(request.id);
      if (updated) {
        await this.modalService.alert({
          title: 'Success',
          message: 'Tracking request submitted successfully!',
          variant: 'success'
        });
        this.loadRequests();
      }
    }
  }

  async onSaveRequest(dto: CreateTrackingRequestDto): Promise<void> {
    if (this.editingRequest) {
      // Update existing
      const updated = this.trackingRequestsService.updateTrackingRequest(this.editingRequest.id, dto);
      if (updated) {
        await this.modalService.alert({
          title: 'Success',
          message: 'Tracking request updated successfully!',
          variant: 'success'
        });
      }
    } else {
      // Create new
      this.trackingRequestsService.createTrackingRequest(dto);
      await this.modalService.alert({
        title: 'Success',
        message: 'Tracking request created successfully!',
        variant: 'success'
      });
    }

    this.closeModal();
    this.loadRequests();
  }

  closeModal(): void {
    this.showCreateModal = false;
    this.editingRequest = undefined;
    setTimeout(() => this.cdr.detectChanges(), 0);
  }
}
