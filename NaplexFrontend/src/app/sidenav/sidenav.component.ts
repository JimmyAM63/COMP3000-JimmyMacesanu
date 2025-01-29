import { Component, EventEmitter, HostListener, OnInit, Output, OnDestroy } from '@angular/core';
import { navbarData } from './nav-data';
import { Router, NavigationEnd, NavigationStart } from '@angular/router';
import { trigger, style, animate, transition, keyframes } from '@angular/animations';
import { AuthService } from '../auth.service'; // Make sure this path is correct!
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';



interface SideNavToggle {
  screenWidth: number;
  collapsed: boolean;
}

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss'],
  animations: [
    trigger('rotate', [
      transition(':enter', [
        animate('1000ms', 
          keyframes([
            style({transform: 'rotate(0deg)', offset: '0'}),
            style({transform: 'rotate(2turn)', offset: '1'})
          ])
        )
      ])
    ])
  ]
})
export class SidenavComponent implements OnInit, OnDestroy {

  isLoginPage = false;
  ifIsRootPage = false;
  showSidenav: boolean = false; // This determines if the sidenav should be displayed based on authentication status

  private subscriptions: Subscription = new Subscription(); // To manage subscriptions

  constructor(private authService: AuthService, private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.isLoginPage = event.url === '/login';
        this.ifIsRootPage = event.url === '/';
      }
    });
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any){
    this.screenWidth = window.innerWidth;
    if(this.screenWidth <= 768){
      this.collapsed = false;
      this.onToggleSideNav.emit({collapsed: this.collapsed, screenWidth: this.screenWidth});
    }
  }

  @Output() onToggleSideNav: EventEmitter<SideNavToggle> = new EventEmitter();
  collapsed = false;
  screenWidth = 0;
  navData = navbarData;

  toggleCollapse(): void {
    this.collapsed = !this.collapsed;
    this.onToggleSideNav.emit({collapsed: this.collapsed, screenWidth: this.screenWidth});
  }

  closeSidenav(): void {
    this.collapsed = false;
    this.onToggleSideNav.emit({collapsed: this.collapsed, screenWidth: this.screenWidth});
  }


  ngOnInit(): void {
    this.subscriptions.add(
      this.router.events.subscribe((event: any) => {
        if (event instanceof NavigationEnd) {
          this.updateSidenavVisibility(event.url);
        }
      })
    );
  
    // Subscribe to changes in authentication status
    this.subscriptions.add(
      this.authService.isAuthenticated$.subscribe(isAuthenticated => {
        if (isAuthenticated) {
          this.updateSidenavVisibility(this.router.url);
        }
      })
    );
  }
  
  updateSidenavVisibility(url: string): void {
    this.showSidenav = url !== '/login';
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe(); // Cleanup subscriptions
  }

  // ... (rest of your existing methods and logic)
}



