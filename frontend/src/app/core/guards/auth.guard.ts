import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { MsalService } from '@azure/msal-angular';

export const authGuard: CanActivateFn = async (route, state) => {
  const msalService = inject(MsalService);

  try {
    // Wait for MSAL to initialize
    await msalService.instance.initialize();

    const isAuthenticated = msalService.instance.getAllAccounts().length > 0;

    if (!isAuthenticated) {
      await msalService.instance.loginRedirect({
        scopes: ['User.Read']
      });
      return false;
    }

    return true;
  } catch (error) {
    console.error('Auth guard error:', error);
    return false;
  }
};

export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return async (route, state) => {
    const msalService = inject(MsalService);
    const router = inject(Router);

    try {
      // Wait for MSAL to initialize
      await msalService.instance.initialize();

      const accounts = msalService.instance.getAllAccounts();

      if (accounts.length === 0) {
        await msalService.instance.loginRedirect({
          scopes: ['User.Read']
        });
        return false;
      }

      const account = accounts[0];
      const roles = account.idTokenClaims?.['roles'] as string[] || [];

      const hasRole = allowedRoles.some(role => roles.includes(role));

      if (!hasRole) {
        router.navigate(['/unauthorized']);
        return false;
      }

      return true;
    } catch (error) {
      console.error('Role guard error:', error);
      router.navigate(['/unauthorized']);
      return false;
    }
  };
};
