import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import apiUrl from './api';


@Injectable({
  providedIn: 'root'
})
export class RotaService {

  constructor(private http: HttpClient) { }

  getStores(): Observable<any> {
    return this.http.get(`${apiUrl}/Stores/`);
  }

  getEmployees(storeId: string): Observable<any> {
    return this.http.get(`${apiUrl}/Stores/${storeId}/employees`);
  }

  getStaffByStore(storeId: number): Observable<any> {
    return this.http.get(`${apiUrl}/Staff/stores/${storeId}/users`);
  }  

  getRotaByStore(storeId: number): Observable<any> {
    return this.http.get<any[]>(`${apiUrl}/Rota/store/${storeId}`).pipe(
      map(rotas => {
        return rotas.map(rota => ({
          ...rota,
          isLeave: rota.isLeave
        }));
      })
    );
  }

  addShift(shiftData: any): Observable<any> {
    return this.http.post<any>(`${apiUrl}/Rota`, shiftData);
  }

  deleteShift(rotaId: number): Observable<any> {
    return this.http.delete<any>(`${apiUrl}/Rota/${rotaId}`);
  }
}
