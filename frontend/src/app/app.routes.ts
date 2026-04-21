import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layouts/main-layout/main-layout';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(m => m.Dashboard),
        canActivate: [authGuard]
      },
      {
        path: 'activities',
        loadComponent: () => import('./features/activities/activities').then(m => m.Activities),
        canActivate: [authGuard]
      },
      {
        path: 'tracking-requests',
        loadComponent: () => import('./features/tracking-requests/tracking-requests').then(m => m.TrackingRequests),
        canActivate: [authGuard]
      },
      {
        path: 'assignments',
        loadComponent: () => import('./features/assignments/assignments').then(m => m.Assignments),
        canActivate: [authGuard]
      },
      {
        path: 'reviews',
        loadComponent: () => import('./features/reviews/reviews').then(m => m.Reviews),
        canActivate: [authGuard]
      },
      {
        path: 'settings',
        loadComponent: () => import('./features/settings/settings').then(m => m.Settings),
        canActivate: [authGuard]
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
