import { Injectable, inject } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { AccountInfo } from '@azure/msal-browser';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from './api.service';
import { User, UserRole } from '../../shared/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private msalService = inject(MsalService);
  private apiService = inject(ApiService);

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {}

  // NEW: Set user from MSAL account (without backend)
  setUserFromMsalAccount(account: AccountInfo | null): void {
    if (account) {
      // Parse name into firstName and lastName
      const fullName = account.name || account.username;
      const nameParts = fullName.split(' ');
      const firstName = nameParts[0] || '';
      const lastName = nameParts.slice(1).join(' ') || '';

      const user: User = {
        id: account.localAccountId,
        entraObjectId: account.localAccountId,
        email: account.username,
        firstName: firstName,
        lastName: lastName,
        role: UserRole.Entity, // Default role - will be updated when backend is available
        entityId: undefined,
        isActive: true,
        createdAt: new Date(),
        updatedAt: new Date()
      };
      
      console.log('👤 User set from MSAL:', user);
      this.currentUserSubject.next(user);
    } else {
      this.currentUserSubject.next(null);
    }
  }

  // Backend sync (use this when backend is running)
  syncCurrentUser(): Observable<User> {
    return this.apiService.post<User>('users/sync-current-user', {}).pipe(
      tap(user => this.currentUserSubject.next(user))
    );
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.msalService.instance.getAllAccounts().length > 0;
  }

  hasRole(role: UserRole): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  hasAnyRole(roles: UserRole[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.includes(user.role) : false;
  }

  login(): void {
    this.msalService.loginRedirect();
  }

  logout(): void {
    this.currentUserSubject.next(null);
    this.msalService.logoutRedirect();
  }
}