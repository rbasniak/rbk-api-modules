import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NgxSmzFormsModule } from 'ngx-smz-dialogs';
import { SmzRouteData } from 'ngx-smz-ui';
import { ButtonModule } from 'primeng/button';
import { LoginComponent } from './login.component';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '0em',
  },
  title: 'Login',
  appArea: 'login',
  clearReusableRoutes: true,
  requiredStates: []
};

const routes: Routes = [
  {
    path: '',
    component: LoginComponent,
    data
  },
];

export const routerModuleForChildLoginModule = RouterModule.forChild(routes);

@NgModule({
  declarations: [LoginComponent],
  imports: [
    CommonModule,
    routerModuleForChildLoginModule,
    ButtonModule,
    NgxSmzFormsModule,
  ],
  exports: []
})
export class LoginModule {
}
