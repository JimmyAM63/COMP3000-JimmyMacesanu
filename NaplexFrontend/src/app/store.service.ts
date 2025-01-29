import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Employee } from './employee.model';
import apiUrl from './api';


interface Store {
  id: number;
  storeName: string;
}

@Injectable({
  providedIn: 'root'
})
export class StoreService {

  constructor(private http: HttpClient) { }

  getStores(): Observable<Store[]> {
    return this.http.get<Store[]>(`${apiUrl}/Stores`);
  }

  getEmployeesByStore(storeId: number): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${apiUrl}/Staff/Stores/${storeId}/users`);
  }

}
