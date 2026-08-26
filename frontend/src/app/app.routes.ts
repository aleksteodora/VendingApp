import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { UserList } from './components/user-list/user-list';
import { AdminShell } from './components/admin-shell/admin-shell';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  {
    path: 'admin',
    component: AdminShell,
    canActivate: [authGuard],
    children: [
      { path: 'users', component: UserList },
      { path: '', redirectTo: 'users', pathMatch: 'full' }
    ]
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];