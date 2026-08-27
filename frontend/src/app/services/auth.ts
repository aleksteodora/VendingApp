import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';

export interface AdminModel {
  id: number;
  email: string;
  fullName: string;
  role: 'Admin' | 'SuperAdmin';
}

export interface LoginResponseModel {
  token: string;
  admin: AdminModel;
}

interface ResponsePackage<T> {
  status: number;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7142/api/auth';

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<LoginResponseModel> {
    return this.http
      .post<ResponsePackage<LoginResponseModel>>(`${this.baseUrl}/login`, { email, password })
      .pipe(
        map(response => response.data),
        tap(data => {
          localStorage.setItem('token', data.token);
          localStorage.setItem('admin', JSON.stringify(data.admin));
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('admin');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getAdmin(): AdminModel | null {
    const adminJson = localStorage.getItem('admin');
    return adminJson ? JSON.parse(adminJson) : null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isSuperAdmin(): boolean {
    return this.getAdmin()?.role === 'SuperAdmin';
  }

  changePassword(currentPassword: string, newPassword: string): Observable<void> {
  return this.http
    .post<ResponsePackage<void>>('https://localhost:7142/api/admin/change-password', {
      currentPassword,
      newPassword
    })
    .pipe(map(() => undefined));
}
}