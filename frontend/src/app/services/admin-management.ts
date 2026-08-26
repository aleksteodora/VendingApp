import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface AdminManagementModel {
  id?: number;
  email: string;
  password?: string;
  fullName: string;
  role?: 'Admin' | 'SuperAdmin';
}

interface ResponsePackage<T> {
  status: number;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class AdminManagementService {
  private baseUrl = 'https://localhost:7142/api/admin';

  constructor(private http: HttpClient) { }

  getAll(): Observable<AdminManagementModel[]> {
    return this.http
      .get<ResponsePackage<AdminManagementModel[]>>(this.baseUrl)
      .pipe(map(response => response.data));
  }

  create(admin: AdminManagementModel): Observable<AdminManagementModel> {
    return this.http
      .post<ResponsePackage<AdminManagementModel>>(this.baseUrl, admin)
      .pipe(map(response => response.data));
  }

  update(id: number, admin: AdminManagementModel): Observable<AdminManagementModel> {
    return this.http
      .put<ResponsePackage<AdminManagementModel>>(`${this.baseUrl}/${id}`, admin)
      .pipe(map(response => response.data));
  }

  delete(id: number): Observable<void> {
    return this.http
      .delete<ResponsePackage<void>>(`${this.baseUrl}/${id}`)
      .pipe(map(() => undefined));
  }
}