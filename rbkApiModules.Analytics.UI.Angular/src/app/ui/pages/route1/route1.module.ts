import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NgxSmzFormsModule } from 'ngx-smz-dialogs';
import { SmzRouteData } from 'ngx-smz-ui';
import { ButtonModule } from 'primeng/button';
import { Route1Component } from './route1.component';

const data: SmzRouteData = {
  layout: {
    mode: 'layout',
    contentPadding: '0px'
  },
  title: 'Route 1',
  appArea: '',
  clearReusableRoutes: true,
  requiredStates: []
};

const routes: Routes = [
  {
    path: '',
    component: Route1Component,
    data
  },
];

export const routerModuleForChildRoute1Module = RouterModule.forChild(routes);

@NgModule({
  declarations: [Route1Component],
  imports: [
    CommonModule,
    routerModuleForChildRoute1Module,
    ButtonModule,
    NgxSmzFormsModule,
  ],
  exports: []
})
export class Route1Module { }
