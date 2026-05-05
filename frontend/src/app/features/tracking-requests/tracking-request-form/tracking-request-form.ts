import { Component, OnInit, Output, EventEmitter, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TrackingRequest, CreateTrackingRequestDto } from '../../../shared/models/tracking-request.model';
import { Frequency } from '../../../shared/models/frequency.model';
import { Entity } from '../../../shared/models/entity.model';
import { FlatpickrDirective } from '../../../shared/directives/flatpickr';

@Component({
  selector: 'app-tracking-request-form',
  standalone: true,
  imports: [CommonModule, FormsModule, FlatpickrDirective],
  templateUrl: './tracking-request-form.html',
  styleUrls: ['./tracking-request-form.css']
})
export class TrackingRequestForm implements OnInit {
  @Input() request?: TrackingRequest;
  @Output() save = new EventEmitter<CreateTrackingRequestDto>();
  @Output() cancel = new EventEmitter<void>();

  // Form model
  formData = {
    title: '',
    description: '',
    goalType: '',
    targetEntityIds: [] as string[],
    startDate: null as Date | string | null,
    dueDate: null as Date | string | null,
    frequency: undefined as Frequency | undefined,
    isRecurring: false
  };

  errors: { [key: string]: string } = {};
  isSubmitting = false;

  // Enums for dropdowns
  Frequency = Frequency;
  frequencies = Object.values(Frequency);

  // Mock entities
  entities: Entity[] = [
    { id: 'entity1', name: 'Finance Department', code: 'FIN', description: '', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity2', name: 'HR Department', code: 'HR', description: '', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity3', name: 'IT Department', code: 'IT', description: '', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity4', name: 'Marketing Department', code: 'MKT', description: '', isActive: true, createdAt: new Date(), updatedAt: new Date() },
    { id: 'entity5', name: 'Operations Department', code: 'OPS', description: '', isActive: true, createdAt: new Date(), updatedAt: new Date() }
  ];

  suggestedGoalTypes = ['Financial', 'HR', 'IT', 'Marketing', 'Operations', 'Safety', 'Compliance', 'Quality'];

  ngOnInit(): void {
    if (this.request) {
      this.formData = {
        title: this.request.title,
        description: this.request.description,
        goalType: this.request.goalType,
        targetEntityIds: [...this.request.targetEntityIds],
        startDate: new Date(this.request.startDate),
        dueDate: new Date(this.request.dueDate!),
        frequency: this.request.frequency,
        isRecurring: this.request.isRecurring
      };
    } else {
      // Set default start date to today
      this.formData.startDate = new Date();
    }
  }

  onStartDateChange(date: Date | null): void {
    this.formData.startDate = date;
  }

  onDueDateChange(date: Date | null): void {
    this.formData.dueDate = date;
  }

 onRecurringChange(): void {
    if (!this.formData.isRecurring) {
      this.formData.frequency = undefined;
      // Clear frequency error when hiding the field
      delete this.errors['frequency'];
    }
  }

  toggleEntitySelection(entityId: string): void {
    const index = this.formData.targetEntityIds.indexOf(entityId);
    if (index > -1) {
      this.formData.targetEntityIds.splice(index, 1);
    } else {
      this.formData.targetEntityIds.push(entityId);
    }
  }

  isEntitySelected(entityId: string): boolean {
    return this.formData.targetEntityIds.includes(entityId);
  }

  validate(): boolean {
    this.errors = {};

    if (!this.formData.title || this.formData.title.trim().length === 0) {
      this.errors['title'] = 'Title is required';
    }

    if (!this.formData.description || this.formData.description.trim().length === 0) {
      this.errors['description'] = 'Description is required';
    }

    if (!this.formData.goalType || this.formData.goalType.trim().length === 0) {
      this.errors['goalType'] = 'Goal type is required';
    }

    if (this.formData.targetEntityIds.length === 0) {
      this.errors['targetEntityIds'] = 'At least one target entity is required';
    }

    if (!this.formData.startDate) {
      this.errors['startDate'] = 'Start date is required';
    }

    if (!this.formData.dueDate) {
      this.errors['dueDate'] = 'Due date is required';
    }

    if (this.formData.startDate && this.formData.dueDate) {
      if (this.formData.dueDate < this.formData.startDate) {
        this.errors['dueDate'] = 'Due date must be after start date';
      }
    }

    if (this.formData.isRecurring && !this.formData.frequency) {
      this.errors['frequency'] = 'Frequency is required for recurring requests';
    }

    return Object.keys(this.errors).length === 0;
  }

  onSubmit(): void {
    if (!this.validate()) {
      return;
    }

    this.isSubmitting = true;

    const dto: CreateTrackingRequestDto = {
      title: this.formData.title,
      description: this.formData.description,
      goalType: this.formData.goalType,
      targetEntityIds: this.formData.targetEntityIds,
      startDate: this.formData.startDate as Date,
      dueDate: this.formData.dueDate as Date,
      frequency: this.formData.frequency,
      isRecurring: this.formData.isRecurring
    };

    this.save.emit(dto);
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
