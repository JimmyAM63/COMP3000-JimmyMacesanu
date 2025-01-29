import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const isLoggedIn = this.authService.isLoggedIn();
    const isTokenExpired = this.authService.isTokenExpired();
    const isAdmin = this.authService.isAdmin(); // fetch the user's role status

    if (!isLoggedIn || isTokenExpired) {
      // Redirect to login page
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
      return false;
    }

    // If you are trying to access the /panel route and you are not an admin
    if (state.url === '/panel' && !isAdmin) {
      this.router.navigate(['/error'], { queryParams: { notAdmin: true }});
      return false;
    }

    return true;
  }
}
