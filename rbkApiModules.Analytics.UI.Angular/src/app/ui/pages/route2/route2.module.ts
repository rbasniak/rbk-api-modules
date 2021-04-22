import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NgxSmzFormsModule } from 'ngx-smz-dialogs';
import { SmzRouteData } from 'ngx-smz-ui';
import { ButtonModule } from 'primeng/button';
import { Route2Component } from './route2.component';

const data: SmzRouteData = {
  layout: {
    mode: 'layout',
    contentPadding: '2em'
  },
  title: 'Route 1',
  appArea: '',
  clearReusableRoutes: true,
  requiredStates: []
};

const routes: Routes = [
  {
    path: '',
    component: Route2Component,
    data
  },
];

export const routerModuleForChildRoute2Module = RouterModule.forChild(routes);

@NgModule({
  declarations: [Route2Component],
  imports: [
    CommonModule,
    routerModuleForChildRoute2Module,
    ButtonModule,
    NgxSmzFormsModule,
  ],
  exports: []
})
export class Route2Module { }
