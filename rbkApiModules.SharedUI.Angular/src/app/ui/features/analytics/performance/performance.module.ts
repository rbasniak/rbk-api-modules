import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgVarDirective, NgVarModule, NgxSmzTablesModule, RbkDatabaseStateGuard, SmzChartModule, SmzRouteData } from 'ngx-smz-ui';
import { NgxSmzFormsModule } from 'ngx-smz-ui';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { ToolbarModule } from 'primeng/toolbar';
import { ANALYTICS_PATH, PERFORMANCE_PATH } from 'src/routes';
import { PerformancePageComponent } from './page/performance-page.component';
import { ANALYTICS_FILTERING_OPTIONS_STATE_NAME } from '@state/database/analytics/filtering-options/filtering-options.state';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '2em'
  },
  title: 'Performance',
  appArea: 'performance',
  clearReusableRoutes: true,
  requiredStates: [ ANALYTICS_FILTERING_OPTIONS_STATE_NAME ]
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: ANALYTICS_PATH + '/' + PERFORMANCE_PATH,
        component: PerformancePageComponent,
        canActivate: [ RbkDatabaseStateGuard ],
        data,
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
    NgVarModule,
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