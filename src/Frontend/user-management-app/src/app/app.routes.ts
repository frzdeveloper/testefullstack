import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },
  {
    path: 'auth/login',
    loadComponent: () =>
      import('./auth/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'users/create',
    loadComponent: () =>
      import('./users/user-create/user-create.component').then(
        (m) => m.UserCreateComponent
      ),
  },
  {
    path: 'users',
    loadComponent: () =>
      import('./users/user-list/user-list.component').then(
        (m) => m.UserListComponent
      ),
    canActivate: [AuthGuard],
  },
  { path: '**', redirectTo: '/auth/login' },
];
