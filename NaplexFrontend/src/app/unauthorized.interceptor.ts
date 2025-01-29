// unauthorized.interceptor.ts
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor,
    HttpResponse,
    HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class UnauthorizedInterceptor implements HttpInterceptor {

    constructor(private router: Router, private authService: AuthService) { }

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                // Checking if it is an Unauthorized error
                if (error.status === 401) {
                    // You can add a condition here to see if the 'Token-Expired' header is true
                    if (this.authService.isTokenExpired()) {
                        this.authService.handleExpiredToken();
                    }
                }
                return throwError(error);
            })
        );
    }
}
