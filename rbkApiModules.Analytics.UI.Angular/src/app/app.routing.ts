import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SearchModule } from '@features/search/search.module';
import { RbkAuthGuard } from 'ngx-rbk-utils';
import { DASHBOARD_PATH, HOME_PATH, SEARCH_PATH } from 'src/routes';
import { DashboardModule } from './ui/features/dashboard/dashboard.module';
import { HomeModule } from './ui/features/home/home.module';

const routes: Routes = [
  {
    path: HOME_PATH,
    loadChildren: (): Promise<HomeModule> => import('./ui/features/home/home.module').then(m => m.HomeModule)
  },
  // {
  //   path: LOGIN_PAGE_PATH,
  //   loadChildren: (): Promise<LoginModule> => import('./ui/pages/login/login.module').then(m => m.LoginModule)
  // },
  {
    path: SEARCH_PATH,
    // canActivate: [ RbkAuthGuard ],
    loadChildren: (): Promise<SearchModule> => import('./ui/features/search/search.module').then(m => m.SearchModule)
  },
  {
    path: DASHBOARD_PATH,
    // canActivate: [ RbkAuthGuard ],
    loadChildren: (): Promise<DashboardModule> => import('./ui/features/dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  // {
  //   path: ROUTE2_PATH,
  //   loadChildren: (): Promise<Route2Module> => import('./ui/pages/route2/route2.module').then(m => m.Route2Module)
  // },
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