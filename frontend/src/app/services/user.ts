import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserModel {
  id?: number;
  fullName: string;
  address: string;
  phoneNumber: string;
  meterSerialNumber: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'https://localhost:7142/api/user';

  constructor(private http: HttpClient) { }

  getAll(): Observable<UserModel[]> {
    return this.http.get<UserModel[]>(this.baseUrl);
  }

  getById(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.baseUrl}/${id}`);
  }

  create(user: UserModel): Observable<UserModel> {
    return this.http.post<UserModel>(this.baseUrl, user);
  }

  update(id: number, user: UserModel): Observable<UserModel> {
    return this.http.put<UserModel>(`${this.baseUrl}/${id}`, user);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}