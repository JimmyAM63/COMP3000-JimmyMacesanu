import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import jwtDecode from 'jwt-decode';
import apiUrl from './api';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private _isAuthenticated = new BehaviorSubject<boolean>(this.isLoggedIn() && !this.isTokenExpired());
  public isAuthenticated$ = this._isAuthenticated.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    if (this.isTokenExpired()) {
      this.handleExpiredToken();
    }
  }

  setLoggedInStatus(status: boolean): void {
    this._isAuthenticated.next(status);
  }

  logout(): void {
    localStorage.removeItem('access_token');
    this._isAuthenticated.next(false);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('access_token'); // True if token exists, false otherwise
  }

  login(credentials: { username: string, password: string }): Observable<any> {
    return this.http.post(`${apiUrl}/Auth/login`, credentials, { withCredentials: true }).pipe(
      tap(() => {
        this._isAuthenticated.next(true); // Setting authentication status to true upon successful login
      })
    );
  }

  getAuthenticatedUserName(): string {
    const token = localStorage.getItem('access_token');
    if (!token) return ''; // Return empty string if token is not available

    const decodedToken: any = jwtDecode(token);
    return decodedToken.unique_name; // Assuming the username is stored in the token
  }

  registerEmployee(employeeData: any): Observable<any> {
    // Setting Authorization header with the JWT token
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders().set('Authorization', 'Bearer ' + token);
    
    return this.http.post(`${apiUrl}/Auth/register`, employeeData, { headers: headers });
  }

  removeEmployee(employeeId: string): Observable<any> {
    // Replace the URL with the appropriate endpoint for removing an employee
    const token = localStorage.getItem('access_token');
    const headers = new HttpHeaders().set('Authorization', 'Bearer ' + token);
    return this.http.delete(`${apiUrl}/Staff/${employeeId}`, { headers: headers });

  }

  isTokenExpired(): boolean {
    const token = localStorage.getItem('access_token');
    if (!token) return true; // No token is technically "expired"
    
    const decodedToken: any = jwtDecode(token);
    const currentTime = new Date().getTime() / 1000;

    return decodedToken.exp < currentTime;
  }

  isAdmin(): boolean {
    const token = localStorage.getItem('access_token');
    if (!token) return false; // If there's no token, user is not an admin
    
    const decodedToken: any = jwtDecode(token);
    
    // Check if the token's role attribute is 'Admin'
    return decodedToken.role === 'Admin';
  }

  handleExpiredToken(): void {
    this.logout();
  }

  lendEmployee(userId: string | number, storeId: string | number): Observable<any> {
    const token = localStorage.getItem('access_token');
    if (!token) {
      // Handle the case when the token is not available. 
      // You can throw an error, or return an observable error.
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${apiUrl}/Staff/users/${userId}/assign-store/${storeId}`, {}, { headers: headers, responseType: 'text' });

  }

  promoteEmployee(userId: string, roleId: string) {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem('access_token')}`,
      'Content-Type': 'application/json'
    });
    const body = {
      userId: userId,
      newRoleId: roleId
    };
  
    // Change this line to use PUT, and adjust the URL if needed based on your API's endpoints.
    return this.http.put(`${apiUrl}/Staff/changeRole`, body, { headers: headers, responseType: 'text' });
  }
}
