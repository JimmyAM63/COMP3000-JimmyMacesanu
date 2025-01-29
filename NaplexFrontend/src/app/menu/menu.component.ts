import { Component, OnInit } from '@angular/core';
import { JwtService } from './jwt.service';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService } from '../auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {
  public firstName: string = '';
  public userRole: string = ' ';
  showMenu: boolean = true;
  isLoginPage: boolean = false;
  private subscriptions: Subscription = new Subscription(); // To manage subscriptions

  constructor(private authService: AuthService, private jwtService: JwtService, private router: Router) { }

  ngOnInit(): void {
      // Subscription for router events
      this.subscriptions.add(
          this.router.events.subscribe(event => {
              if (event instanceof NavigationEnd) {
                  this.updateNameFromToken();
                  this.isLoginPage = event.url === '/login';
                  this.updateMenuVisibility(this.router.url);
              }
          })
      );
      this.updateNameFromToken();  // For the initial load
      
      // Subscription for authentication state
      this.subscriptions.add(
        this.authService.isAuthenticated$.subscribe(isAuthenticated => {
          if (isAuthenticated) {
            this.updateMenuVisibility(this.router.url);
          }
        })
      );
  }

  updateMenuVisibility(url: string): void {
    this.showMenu = url !== '/login';
  }

  ngOnDestroy(): void {
      this.subscriptions.unsubscribe(); // Cleanup subscriptions
  }

  updateNameFromToken(): void {
      const token = localStorage.getItem('access_token');
      if (token) {
          const decodedToken = this.jwtService.decodeToken(token);
          this.firstName = decodedToken?.unique_name || '';
          this.userRole = decodedToken?.role || '';
      }
  }

  logout() {
      this.authService.logout();
      this.router.navigate(['/login']);
      this.authService.setLoggedInStatus(false); // Set the login status to false
  }
}