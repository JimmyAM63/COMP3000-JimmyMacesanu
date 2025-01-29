import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import apiUrl from './api';

@Injectable({
  providedIn: 'root'
})
export class TargetService {

  private baseUrl = `${apiUrl}/Targets`;

  constructor(private http: HttpClient) { }

  getTargetsByUserAndMonth(userId: string, monthYear: Date): Observable<any[]> {
    // Format monthYear to "YYYY-MM" string
    const monthYearStr = `${monthYear.getFullYear()}-${('0' + (monthYear.getMonth() + 1)).slice(-2)}`;
    return this.http.get<any[]>(`${this.baseUrl}/user/${userId}/${monthYearStr}`);
  }

  getTargetsByStoreAndMonth(storeId: string, monthYear: Date): Observable<any[]> {
    // Format monthYear to "YYYY-MM" string
    const monthYearStr = `${monthYear.getFullYear()}-${('0' + (monthYear.getMonth() + 1)).slice(-2)}`;
    return this.http.get<any[]>(`${this.baseUrl}/store/${storeId}/${monthYearStr}`);
  }

  updateTarget(targetId: number, targetData: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${targetId}`, targetData);
  }

  createTarget(userId: string, storeId: number, monthYear: string, newTar: number, talkMobileTar: number,
    additionalTar: number,  upgradesTar: number, hbbTar: number, hbbUpTar: number, revTar: number, unlimitedTar: number, insuranceTar: number): Observable<any> {
    const body = {
      userId,
      storeId,
      monthYear,
      newTar,
      talkMobileTar,
      additionalTar,
      upgradesTar, 
      hbbTar, 
      hbbUpTar, 
      revTar, 
      unlimitedTar,
      insuranceTar
    };
    return this.http.post<any>(this.baseUrl, body);
  }
  
}