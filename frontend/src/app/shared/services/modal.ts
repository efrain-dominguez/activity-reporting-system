import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ConfirmationState {
  isVisible: boolean;
  title: string;
  message: string;
  confirmText: string;
  cancelText: string;
  variant: 'info' | 'warning' | 'error' | 'success';
}

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  confirmationState: ConfirmationState = {
    isVisible: false,
    title: '',
    message: '',
    confirmText: 'Confirm',
    cancelText: 'Cancel',
    variant: 'info'
  };

  private confirmSubject = new Subject<boolean>();

  confirm(options: {
    title: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
    variant?: 'info' | 'warning' | 'error' | 'success';
  }): Promise<boolean> {
    this.confirmationState = {
      isVisible: true,
      title: options.title,
      message: options.message,
      confirmText: options.confirmText || 'Confirm',
      cancelText: options.cancelText || 'Cancel',
      variant: options.variant || 'info'
    };

    return new Promise((resolve) => {
      const subscription = this.confirmSubject.subscribe((result) => {
        subscription.unsubscribe();
        resolve(result);
      });
    });
  }

  alert(options: {
    title: string;
    message: string;
    confirmText?: string;
    variant?: 'info' | 'warning' | 'error' | 'success';
  }): Promise<void> {
    this.confirmationState = {
      isVisible: true,
      title: options.title,
      message: options.message,
      confirmText: options.confirmText || 'OK',
      cancelText: '',
      variant: options.variant || 'info'
    };

    return new Promise((resolve) => {
      const subscription = this.confirmSubject.subscribe(() => {
        subscription.unsubscribe();
        resolve();
      });
    });
  }

  confirmAction(): void {
    this.confirmationState.isVisible = false;
    this.confirmSubject.next(true);
  }

  cancelAction(): void {
    this.confirmationState.isVisible = false;
    this.confirmSubject.next(false);
  }
}
