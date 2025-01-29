import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import apiUrl from './api';

@Injectable({
  providedIn: 'root'
})
export class SalesService {

  constructor(private http: HttpClient) { }

  getAllSales(): Observable<any[]> {
    return this.http.get<any[]>(`${apiUrl}/Sales`);
  }

  deleteSale(saleId: number): Observable<any> {
    const url = `${apiUrl}/Sales/${saleId}`;
    return this.http.delete<any>(url);
  }

  addSale(saleData: any): Observable<any> {
    return this.http.post<any>(`${apiUrl}/Sales`, saleData);
  }

  getAdvisorStore(userId: string) {
    return this.http.get<any[]>(`${apiUrl}/Stores/user/${userId}/stores`);
  }

  // Method to fetch staff name by userId
  getStaffName(userId: string): Observable<any> {
    return this.http.get<any>(`${apiUrl}/Staff/${userId}`);
  }
}
