import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HttpClientModule } from '@angular/common/http';
import FOG from 'vanta/dist/vanta.fog.min';
import { BodyComponent } from './body/body.component';
import { MatIconModule } from '@angular/material/icon';
import { SidenavComponent } from './sidenav/sidenav.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TargetsComponent } from './targets/targets.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { SalesComponent } from './sales/sales.component';
import { RotaComponent } from './rota/rota.component';
import { SettingsComponent } from './settings/settings.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { StaffComponent } from './staff/staff.component';
import { StoreSelectionModalComponent } from './store-selection-modal/store-selection-modal.component';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { UnauthorizedInterceptor } from './unauthorized.interceptor';
import { MenuComponent } from './menu/menu.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NotificationComponent } from './notification/notification.component';
import { PanelComponent } from './panel/panel.component';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { CalendarModule } from 'primeng/calendar';
import { DatePipe } from '@angular/common';
import { ChatComponent } from './chat/chat.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { StackedAreaChartComponent } from './stacked-area-chart/stacked-area-chart.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { MatSelectModule } from '@angular/material/select';

declare global {
  interface Window {
    FOG: any;
  }
}

window.FOG = FOG;


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    BodyComponent,
    SidenavComponent,
    TargetsComponent,
    SalesComponent,
    RotaComponent,
    SettingsComponent,
    StaffComponent,
    StoreSelectionModalComponent,
    LeaderboardComponent,
    MenuComponent,
    NotificationComponent,
    ChatComponent,
    StackedAreaChartComponent,
    PanelComponent
  ],
  imports: [
    BrowserAnimationsModule,
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    DragDropModule,
    FormsModule,
    MatIconModule,
    MatSnackBarModule,
    HttpClientModule,
    MatDatepickerModule,
    MatNativeDateModule,
    CalendarModule,
    DatePipe,
    NgxChartsModule,
    MatSelectModule,
    BsDatepickerModule.forRoot() // Use forRoot() here
  ],
  providers: [
    DatePipe,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UnauthorizedInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
