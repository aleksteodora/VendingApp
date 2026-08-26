import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { UserList } from './components/user-list/user-list';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'admin', component: UserList },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
