import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RbkAuthGuard } from 'ngx-rbk-utils';
import { COURSES_PATH, HOME_PATH, LOGIN_PAGE_PATH, ROUTE2_PATH } from 'src/routes';
import { HomeModule } from './ui/features/home/home.module';
import { LoginModule } from './ui/pages/login/login.module';
import { Route1Module } from './ui/pages/route1/route1.module';
import { Route2Module } from './ui/pages/route2/route2.module';

const routes: Routes = [
  {
    path: HOME_PATH,
    loadChildren: (): Promise<HomeModule> => import('./ui/features/home/home.module').then(m => m.HomeModule)
  },
  // {
  //   path: LOGIN_PAGE_PATH,
  //   loadChildren: (): Promise<LoginModule> => import('./ui/pages/login/login.module').then(m => m.LoginModule)
  // },
  // {
  //   path: COURSES_PATH,
  //   canActivate: [ RbkAuthGuard ],
  //   loadChildren: (): Promise<Route1Module> => import('./ui/features/courses/courses.module').then(m => m.CoursesModule)
  // },
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