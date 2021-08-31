import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SearchModule as AnalyticsSearchModule } from '@features/analytics/search/search.module';
import { SearchModule as DiagnosticsSearchModule} from '@features/diagnostics/search/search.module';
import { MENU_STATE_NAME } from '@state/database/menu/menu.state';
import { RbkAuthGuard, RbkDatabaseStateGuard } from 'ngx-smz-ui';
import { HOME_PATH, LOGIN_PATH } from 'src/routes';
import { DashboardModule as AnalyticsDashboardhModule } from './ui/features/analytics/dashboard/dashboard.module';
import { DashboardModule as DiagnosticsDashboardModule } from './ui/features/diagnostics/dashboard/dashboard.module';
import { AdminModule as DiagnosticsAdminModule } from './ui/features/diagnostics/admin/admin.module';
import { AdminModule as AnalyticsAdminModule } from './ui/features/analytics/admin/admin.module';
import { HomeModule } from './ui/features/home/home.module';
import { PerformanceModule } from './ui/features/analytics/performance/performance.module';
import { SessionsModule } from './ui/features/analytics/sessions/sessions.module';

const routes: Routes = [
  { path: '', canActivate: [RbkDatabaseStateGuard], data: {requiredStates: [MENU_STATE_NAME]}, children: [
    {
      path: HOME_PATH,
      loadChildren: (): Promise<HomeModule> => import('./ui/features/home/home.module').then(m => m.HomeModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<AnalyticsSearchModule> => import('./ui/features/analytics/search/search.module').then(m => m.SearchModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<AnalyticsDashboardhModule> => import('./ui/features/analytics/dashboard/dashboard.module').then(m => m.DashboardModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<AnalyticsAdminModule> => import('./ui/features/analytics/admin/admin.module').then(m => m.AdminModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<DiagnosticsAdminModule> => import('./ui/features/diagnostics/admin/admin.module').then(m => m.AdminModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<DiagnosticsSearchModule> => import('./ui/features/diagnostics/search/search.module').then(m => m.SearchModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<DiagnosticsDashboardModule> => import('./ui/features/diagnostics/dashboard/dashboard.module').then(m => m.DashboardModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<PerformanceModule> => import('./ui/features/analytics/performance/performance.module').then(m => m.PerformanceModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<SessionsModule> => import('./ui/features/analytics/sessions/sessions.module').then(m => m.SessionsModule)
    },
    {
      path: LOGIN_PATH,
      loadChildren: (): Promise<HomeModule> => import('./ui/login/login.module').then(m => m.LoginModule)
    },
  ]},
  {
    path: '',
    redirectTo: HOME_PATH,
    pathMatch: 'full'
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }