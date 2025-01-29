import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
  })
  export class JwtService {
  
    decodeToken(token: string): any {
      try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace('-', '+').replace('_', '/');
        return JSON.parse(atob(base64));
      } catch (error) {
        console.error("Error decoding token", error);
        return null;
      }
    }
  }
  