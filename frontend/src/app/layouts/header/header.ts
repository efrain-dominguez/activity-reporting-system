import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme';
import { SidebarService } from '../../core/services/sidebar';
import { PopperDirective } from '../../shared/directives/popper';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, PopperDirective],
  templateUrl: './header.html',
  styleUrls: ['./header.css']
})
export class HeaderComponent {
  // Modern Angular 21 inject() pattern
  private authService = inject(AuthService);
  private themeService = inject(ThemeService);
  private sidebarService = inject(SidebarService);
  
  currentUser$ = this.authService.currentUser$;
  
  // Activity Reporting System notifications
  notifications = [
    {
      id: 1,
      title: 'New Activity Submitted',
      message: 'John Doe submitted activity report #1234',
      time: '5m ago',
      unread: true,
      iconClass: 'fa-solid fa-file-lines',
      bgClass: 'bg-primary/10 dark:bg-accent-light/15',
      iconColor: 'text-primary dark:text-accent-light'
    },
    {
      id: 2,
      title: 'Review Required',
      message: 'Activity #1235 requires your review',
      time: '1h ago',
      unread: true,
      iconClass: 'fa fa-clipboard-check',
      bgClass: 'bg-success/10 dark:bg-success/15',
      iconColor: 'text-success'
    },
    {
      id: 3,
      title: 'New Assignment',
      message: 'You have been assigned to review 3 activities',
      time: '2h ago',
      unread: true,
      iconClass: 'fa fa-user-edit',
      bgClass: 'bg-info/10 dark:bg-info/15',
      iconColor: 'text-info'
    },
    {
      id: 4,
      title: 'Activity Approved',
      message: 'Your activity report #1230 was approved',
      time: '3h ago',
      unread: false,
      iconClass: 'fa fa-check-circle',
      bgClass: 'bg-success/10 dark:bg-success/15',
      iconColor: 'text-success'
    },
    {
      id: 5,
      title: 'Deadline Approaching',
      message: 'Activity report #1229 is due in 2 days',
      time: '1d ago',
      unread: false,
      iconClass: 'fa fa-exclamation-triangle',
      bgClass: 'bg-warning/10 dark:bg-warning/15',
      iconColor: 'text-warning'
    }
  ];

  login(): void {
    console.log('🔵 Login button clicked!');
    this.authService.login();
  }

  logout(): void {
    console.log('🔴 Logout button clicked!');
    this.authService.logout();
  }

  toggleSidebar(): void {
    this.sidebarService.toggle();
  }

  toggleDarkMode(): void {
    this.themeService.toggleDarkMode();
  }

  markAsRead(notificationId: number): void {
    const notification = this.notifications.find(n => n.id === notificationId);
    if (notification) {
      notification.unread = false;
    }
  }

  get unreadCount(): number {
    return this.notifications.filter(n => n.unread).length;
  }
}