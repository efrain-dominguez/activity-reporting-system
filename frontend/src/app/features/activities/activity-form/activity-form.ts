import { Component, OnInit, Output, EventEmitter, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Activity, CreateActivityDto } from '../../../shared/models/activity.model';
import { FlatpickrDirective } from '../../../shared/directives/flatpickr';

@Component({
  selector: 'app-activity-form',
  standalone: true,
  imports: [CommonModule, FormsModule, FlatpickrDirective],
  templateUrl: './activity-form.html',
  styleUrls: ['./activity-form.css']
})
export class ActivityForm implements OnInit {
  @Input() activity?: Activity; // For editing
  @Output() save = new EventEmitter<CreateActivityDto>();
  @Output() cancel = new EventEmitter<void>();

  // Form model
  formData = {
    assignmentId: '',
    requestId: '',
    description: '',
    activityDate: null as Date | string | null,
  };

  selectedFiles: File[] = [];
  errors: { [key: string]: string } = {};
  isSubmitting = false;

  // Mock data for dropdowns - replace with actual data from API
  assignments = [
    { id: 'assignment1', name: 'Q1 Performance Review' },
    { id: 'assignment2', name: 'Project Kickoff Meeting' },
    { id: 'assignment3', name: 'Budget Planning Q2' }
  ];

  requests = [
    { id: 'request1', name: 'Monthly Report Request' },
    { id: 'request2', name: 'Team Update Request' },
    { id: 'request3', name: 'Financial Analysis Request' }
  ];

  ngOnInit(): void {
    if (this.activity) {
      // Populate form for editing
      this.formData = {
        assignmentId: this.activity.assignmentId,
        requestId: this.activity.requestId,
        description: this.activity.description,
        activityDate: this.formatDateForInput(this.activity.activityDate),
      };
    }
  }

  onActivityDateChange(date: Date | null): void {
    this.formData.activityDate = date;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files);
    }
  }

  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);
  }

  validate(): boolean {
    this.errors = {};

    if (!this.formData.assignmentId) {
      this.errors['assignmentId'] = 'Assignment is required';
    }

    if (!this.formData.requestId) {
      this.errors['requestId'] = 'Request is required';
    }

    if (!this.formData.description || this.formData.description.trim().length === 0) {
      this.errors['description'] = 'Description is required';
    }

    if (!this.formData.activityDate) {
      this.errors['activityDate'] = 'Activity date is required';
    }

    return Object.keys(this.errors).length === 0;
  }

  onSubmit(): void {
    if (!this.validate()) {
      return;
    }

    this.isSubmitting = true;

    const dto: CreateActivityDto = {
      assignmentId: this.formData.assignmentId,
      requestId: this.formData.requestId,
      description: this.formData.description,
      activityDate: new Date(this.formData.activityDate!),
      files: this.selectedFiles
    };

    this.save.emit(dto);
  }

  onCancel(): void {
    this.cancel.emit();
  }

  formatDateForInput(date: Date): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }
}
