import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSmzTablesModule, SmzChartModule, SmzRouteData } from 'ngx-smz-ui';
import { NgxSmzFormsModule } from 'ngx-smz-dialogs';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';
import { ANALYTICS_PATH, PERFORMANCE_PATH } from 'src/routes';
import { PerformancePageComponent } from './page/performance-page.component';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '2em'
  },
  title: 'Performance',
  appArea: 'performance',
  clearReusableRoutes: true,
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: ANALYTICS_PATH + '/' + PERFORMANCE_PATH,
        component: PerformancePageComponent,
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
    PerformancePageComponent
  ],
  providers: [

  ],
})
export class PerformanceModule { }