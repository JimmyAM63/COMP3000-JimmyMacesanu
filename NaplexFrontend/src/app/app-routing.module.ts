import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthGuard } from './auth.guard';
import { TargetsComponent } from './targets/targets.component';
import { SalesComponent } from './sales/sales.component';
import { RotaComponent } from './rota/rota.component';
import { SettingsComponent } from './settings/settings.component';
import { StaffComponent } from './staff/staff.component';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { PanelComponent } from './panel/panel.component';
import { ChatComponent } from './chat/chat.component';


const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'leaderboard', component: LeaderboardComponent, canActivate: [AuthGuard]},
  { path: 'targets', component: TargetsComponent, canActivate: [AuthGuard] },
  { path: 'sales', component: SalesComponent, canActivate: [AuthGuard] },
  { path: 'rota', component: RotaComponent, canActivate: [AuthGuard] },
  { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard]},
  { path: 'staff', component: StaffComponent, canActivate: [AuthGuard]},
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'panel', component: PanelComponent, canActivate: [AuthGuard] },
  { path: 'chat', component: ChatComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
