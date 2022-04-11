import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSmzTablesModule, SmzChartModule, SmzRouteData } from 'ngx-smz-ui';
import { NgxSmzFormsModule } from 'ngx-smz-ui';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { DashboardPageComponent } from './page/dashboard-page.component';
import { ToolbarModule } from 'primeng/toolbar';
import { LOGS_PATH, DASHBOARD_PATH } from 'src/routes';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '2em'
  },
  title: 'Dashboard',
  appArea: 'dashboard',
  clearReusableRoutes: true,
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: LOGS_PATH + '/' + DASHBOARD_PATH,
        component: DashboardPageComponent,
        data
      },
    ]
  },
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ButtonModule,
    NgxSmzFormsModule,
    PanelModule,
    NgxSmzTablesModule,
    ToolbarModule,
    SmzChartModule,
    CardModule
  ],
  exports: [],
  declarations: [
    DashboardPageComponent,
  ],
  providers: [

  ],
})
export class DashboardModule { }