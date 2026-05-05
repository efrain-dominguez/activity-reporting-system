import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { MsalService } from '@azure/msal-angular';
import { ConfirmationModal } from './shared/components/confirmation-modal/confirmation-modal';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, ConfirmationModal],
  template: `
    <router-outlet></router-outlet>
    <app-confirmation-modal></app-confirmation-modal>
  `
})
export class App implements OnInit {
  private authService = inject(AuthService);
  private msalService = inject(MsalService);
  private router = inject(Router);

  title = 'Activity Reporting System';

  async ngOnInit(): Promise<void> {
    console.log('🚀 App initializing...');

    try {
      // Initialize MSAL
      await this.msalService.instance.initialize();
      console.log('✅ MSAL initialized');

      // Handle redirect promise (after login callback)
      const response = await this.msalService.instance.handleRedirectPromise();

      if (response) {
        // User just logged in via redirect - set user and navigate to dashboard
        console.log('✅ Login redirect handled:', response.account?.username);
        this.authService.setUserFromMsalAccount(response.account);

        // Navigate to dashboard after successful login
        console.log('🔄 Navigating to dashboard...');
        await this.router.navigate(['/dashboard']);
      } else {
        // Check if user is already logged in
        const accounts = this.msalService.instance.getAllAccounts();
        if (accounts.length > 0) {
          console.log('✅ User already logged in:', accounts[0].username);
          this.authService.setUserFromMsalAccount(accounts[0]);

          // If on root path, redirect to dashboard
          if (this.router.url === '/' || this.router.url === '') {
            console.log('🔄 Redirecting to dashboard...');
            await this.router.navigate(['/dashboard']);
          }
        } else {
          console.log('ℹ️ No user logged in');
        }
      }
    } catch (error) {
      console.error('❌ MSAL initialization error:', error);
    }
  }
}
