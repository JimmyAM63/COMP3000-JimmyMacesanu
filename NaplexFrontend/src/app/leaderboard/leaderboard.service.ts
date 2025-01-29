import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import apiUrl from '../api';


export interface Store {
  id: number;
  storeName: string;
}

@Injectable({
  providedIn: 'root'
})
export class LeaderboardService {


  constructor(private http: HttpClient) {}

  getStores(): Observable<Store[]> {
    return this.http.get<Store[]>(`${apiUrl}/Stores`);
  }
}
