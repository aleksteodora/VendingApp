import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface UserModel {
  id?: number;
  fullName: string;
  address: string;
  phoneNumber: string;
  meterSerialNumber: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

interface ResponsePackage<T> {
  status: number;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'http://localhost:5245/api/user';

  constructor(private http: HttpClient) { }

  getAll(pageNumber: number, pageSize: number): Observable<PagedResult<UserModel>> {
    return this.http
      .get<ResponsePackage<PagedResult<UserModel>>>(`${this.baseUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`)
      .pipe(map(response => response.data));
  }

  getById(id: number): Observable<UserModel> {
    return this.http.get<ResponsePackage<UserModel>>(`${this.baseUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  create(user: UserModel): Observable<UserModel> {
    return this.http.post<ResponsePackage<UserModel>>(this.baseUrl, user)
      .pipe(map(response => response.data));
  }

  update(id: number, user: UserModel): Observable<UserModel> {
    return this.http.put<ResponsePackage<UserModel>>(`${this.baseUrl}/${id}`, user)
      .pipe(map(response => response.data));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<ResponsePackage<void>>(`${this.baseUrl}/${id}`)
      .pipe(map(() => undefined));
  }
}