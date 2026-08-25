import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { UserList } from './components/user-list/user-list';
import { AdminList } from './components/admin-list/admin-list';
import { ProcessingFeeList } from './components/processing-fee-list/processing-fee-list';
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
      { path: 'processing-fee', component: ProcessingFeeList },
      { path: 'admins', component: AdminList },
      { path: '', redirectTo: 'users', pathMatch: 'full' }
    ]
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];