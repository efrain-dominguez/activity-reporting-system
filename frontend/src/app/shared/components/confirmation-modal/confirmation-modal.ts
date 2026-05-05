import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalService } from '../../services/modal';

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirmation-modal.html',
  styleUrls: ['./confirmation-modal.css']
})
export class ConfirmationModal {
  private modalService = inject(ModalService);

  get isVisible(): boolean {
    return this.modalService.confirmationState.isVisible;
  }

  get title(): string {
    return this.modalService.confirmationState.title;
  }

  get message(): string {
    return this.modalService.confirmationState.message;
  }

  get confirmText(): string {
    return this.modalService.confirmationState.confirmText;
  }

  get cancelText(): string {
    return this.modalService.confirmationState.cancelText;
  }

  get variant(): 'info' | 'warning' | 'error' | 'success' {
    return this.modalService.confirmationState.variant;
  }

  onConfirm(): void {
    this.modalService.confirmAction();
  }

  onCancel(): void {
    this.modalService.cancelAction();
  }

  onBackdropClick(): void {
    this.modalService.cancelAction();
  }

  getIconClass(): string {
    switch (this.variant) {
      case 'error':
        return 'text-error';
      case 'warning':
        return 'text-warning';
      case 'success':
        return 'text-success';
      default:
        return 'text-info';
    }
  }

  getIcon(): string {
    switch (this.variant) {
      case 'error':
        return 'M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'warning':
        return 'M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z';
      case 'success':
        return 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z';
      default:
        return 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z';
    }
  }
}
