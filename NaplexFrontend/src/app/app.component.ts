import { HostListener } from '@angular/core';
import { Component, ElementRef, OnDestroy, OnInit } from '@angular/core';
import * as THREE from 'three';
import BIRDS from 'vanta/dist/vanta.birds.min';
import FOG from 'vanta/dist/vanta.fog.min';
import WAVES from 'vanta/dist/vanta.waves.min';
import CLOUDS from 'vanta/dist/vanta.clouds.min';
import CLOUDS2 from 'vanta/dist/vanta.clouds2.min';
import GLOBE from 'vanta/dist/vanta.globe.min';
import NET from 'vanta/dist/vanta.net.min';
import CELLS from 'vanta/dist/vanta.cells.min';
import TRUNK from 'vanta/dist/vanta.trunk.min';
import TOPOLOGY from 'vanta/dist/vanta.topology.min';
import DOTS from 'vanta/dist/vanta.dots.min';
import RINGS from 'vanta/dist/vanta.rings.min';
import HALO from 'vanta/dist/vanta.halo.min';
import { debounce } from 'lodash';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService } from './auth.service';

const VANTAS = [
  BIRDS,
  FOG,
  WAVES,
  CLOUDS,
  CLOUDS2,
  GLOBE,
  NET,
  CELLS,
  TRUNK,
  TOPOLOGY,
  DOTS,
  RINGS,
  HALO
];

interface SideNavToggle {
  screenWidth: number;
  collapsed: boolean;
}


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'retail-dashboard';
  effect: any;
  private vantaEffect: any;
  private lastN: number;
  private tokenCheckInterval: any;
  showMenu: boolean = true;

  constructor(
    private el: ElementRef,
    private authService: AuthService,
    private router: Router) {
    this.lastN = -1;
    this.authService.isAuthenticated$.subscribe(isAuthenticated => {
      this.showMenu = isAuthenticated;
    });
  }

  isSideNavCollapsed = false;
  screenWidth = 0;

  onToggleSideNav(data: SideNavToggle): void {
    this.screenWidth = data.screenWidth;
    this.isSideNavCollapsed = data.collapsed;
  }

  @HostListener('click')
  protected buildVantaEffect() {

  }

  protected destroyVantaEffect() {
    if (this.vantaEffect) {
      this.vantaEffect.destroy();
    }
  }

  ngOnInit(): void {

    // Check if the user is authenticated and set showMenu accordingly
    

    // Check authentication status when the application starts
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd && event.url === '/') {
        if (this.authService.isLoggedIn()) {
          this.router.navigate(['/dashboard']);
        } else {
          this.router.navigate(['/login']);
        }
      }});

    this.tokenCheckInterval = setInterval(() => {
      if (this.authService.isTokenExpired()) {
        this.authService.handleExpiredToken();
      }
    }, 10000);  // Checks every 10 seconds

    try {
      const vanta = VANTAS[7];
      this.vantaEffect = vanta({
        el: "#vanta",
        mouseControls: true,
  touchControls: true,
  gyroControls: false,
  minHeight: 200.00,
  minWidth: 200.00,
  scale: 1.00,
  scaleMobile: 1.00,
  color: 0xff0000,
  color2: 0xff0000,
  size: 0.50,
  backgroundColor: 0xffffff
      });
      this.vantaEffect.resizes()
    } catch (err) {
      console.error(err);
      this.buildVantaEffect();
    }
  }

  ngOnDestroy(): void {
    this.destroyVantaEffect();

    if (this.tokenCheckInterval) {
      clearInterval(this.tokenCheckInterval);
    }
  }

  @HostListener('click')
protected onHostClick() {
  this.debouncedBuildVantaEffect();
}

protected debouncedBuildVantaEffect = debounce(this.buildVantaEffect, 300);
}
