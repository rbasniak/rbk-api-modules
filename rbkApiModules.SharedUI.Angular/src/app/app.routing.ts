import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SearchModule } from '@features/analytics/search/search.module';
import { MENU_STATE_NAME } from '@state/database/menu/menu.state';
import { RbkAuthGuard, RbkDatabaseStateGuard } from 'ngx-rbk-utils';
import { DASHBOARD_PATH, HOME_PATH, LOGIN_PATH, SEARCH_PATH } from 'src/routes';
import { DashboardModule } from './ui/features/analytics/dashboard/dashboard.module';
import { HomeModule } from './ui/features/home/home.module';

const routes: Routes = [
  { path: '', canActivate: [RbkDatabaseStateGuard], data: {requiredStates: [MENU_STATE_NAME]}, children: [
    {
      path: HOME_PATH,
      loadChildren: (): Promise<HomeModule> => import('./ui/features/home/home.module').then(m => m.HomeModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<SearchModule> => import('./ui/features/analytics/search/search.module').then(m => m.SearchModule)
    },
    {
      path: '',
      canActivate: [ RbkAuthGuard ],
      loadChildren: (): Promise<DashboardModule> => import('./ui/features/analytics/dashboard/dashboard.module').then(m => m.DashboardModule)
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