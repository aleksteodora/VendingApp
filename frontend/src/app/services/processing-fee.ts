import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface ProcessingFeeModel {
  id: number;
  fixedAmount: number;
  percentageRate: number;
  isDeleted: boolean;
  createdAt: string;
}

export interface ProcessingFeeChangeModel {
  fixedAmount: number;
  percentageRate: number;
}

interface ResponsePackage<T> {
  status: number;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class ProcessingFeeService {
  private baseUrl = 'http://localhost:5245/api/processing-fee';

  constructor(private http: HttpClient) { }

  getActive(): Observable<ProcessingFeeModel> {
    return this.http
      .get<ResponsePackage<ProcessingFeeModel>>(`${this.baseUrl}/active`)
      .pipe(map(response => response.data));
  }

  getHistory(): Observable<ProcessingFeeModel[]> {
    return this.http
      .get<ResponsePackage<ProcessingFeeModel[]>>(`${this.baseUrl}/history`)
      .pipe(map(response => response.data));
  }

  change(fee: ProcessingFeeChangeModel): Observable<ProcessingFeeModel> {
    return this.http
      .put<ResponsePackage<ProcessingFeeModel>>(`${this.baseUrl}/change`, fee)
      .pipe(map(response => response.data));
  }
}