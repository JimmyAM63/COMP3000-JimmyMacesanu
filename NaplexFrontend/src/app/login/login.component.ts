import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { HostListener } from '@angular/core';
import { Component, ElementRef, OnDestroy, OnInit } from '@angular/core';
import * as THREE from 'three';
import { JwtService } from '../menu/jwt.service';
import { Subscription } from 'rxjs';
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
import { MatSnackBar } from '@angular/material/snack-bar';

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


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  username: string = '';
  password: string = '';
  rememberMe = false;
  showMenu: boolean = true;
  showNotification = false;
  public firstName: string = '';
  notificationMessage = '';
  isLoadingDashboard: boolean = false;

  effect: any;

  private vantaEffect: any;


  @HostListener('click')
  protected buildVantaEffect() {

  }

  protected destroyVantaEffect() {
    if (this.vantaEffect) {
      this.vantaEffect.destroy();
    }
  }

  constructor(
    private authService: AuthService, 
    private router: Router,
    private snackBar: MatSnackBar, // Injecting MatSnackBar service
    private jwtService: JwtService, 
  ) { }

  updateNameFromToken(): void {
    const token = localStorage.getItem('access_token');
    if (token) {
        const decodedToken = this.jwtService.decodeToken(token);
        this.firstName = decodedToken?.unique_name || '';
    }
  }

  ngOnInit(): void {
    try {
      const vanta = VANTAS[1];
      this.vantaEffect = vanta({
        el: "#vanta",
        THREE: THREE, // use a custom THREE when initializing
        mouseControls: true,
        touchControls: true,
        gyroControls: false,
        minHeight: 200.00,
        minWidth: 200.00,
        highlightColor: 0xff0000,
        midtoneColor: 0xffffff,
        lowlightColor: 0xffffff,
        baseColor: 0xffffff,
        blurFactor: 0.90,
        speed: 0.50,
        zoom: 0.60
      });
      this.vantaEffect.resizes()
    } catch (err) {
      console.error(err);
      this.buildVantaEffect();
    }

    this.updateNameFromToken();
  }

  checkContent() {
    // For now, this function doesn't need to do anything, 
    // but it's here in case you want to extend functionality later.
  }

  ngOnDestroy(): void {
    this.destroyVantaEffect();
  }

  @HostListener('click')
  protected onHostClick() {
    this.debouncedBuildVantaEffect();
  }

  protected debouncedBuildVantaEffect = debounce(this.buildVantaEffect, 300);

  onLogin() {
    this.authService.login({ username: this.username, password: this.password })
        .subscribe(
          data => {
            localStorage.setItem('access_token', data.access_token);
            this.updateNameFromToken();  
            this.showNotificationMessage(`Welcome, ${this.firstName}!`);
            this.isLoadingDashboard = true; // Start loading animation
            this.authService.setLoggedInStatus(true); // Set logged-in status to true
            setTimeout(() => {
              this.router.navigate(['/dashboard']); // Navigate to dashboard after animation
            }, 3000);  // Delay of 3 seconds
          },
          error => {
            this.showNotificationMessage("Credentials unrecognised. Try again");
          }
      );
  }
  
  
  showNotificationMessage(message: string): void {
    this.notificationMessage = message;
    this.showNotification = true;
    setTimeout(() => this.showNotification = false, 3000);  // Hide after 5 seconds
  }
}