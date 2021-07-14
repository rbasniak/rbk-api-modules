import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSmzTablesModule, SmzChartModule, SmzRouteData } from 'ngx-smz-ui';
import { RbkDatabaseStateGuard } from 'ngx-rbk-utils';
import { NgxSmzFormsModule } from 'ngx-smz-dialogs';
import { ButtonModule } from 'primeng/button';
import { SearchPageComponent } from './page/search-page.component';
import { ANALYTICS_FILTERING_OPTIONS_STATE_NAME } from '@state/database/analytics/filtering-options/filtering-options.state';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { ANALYTICS_PATH, SEARCH_PATH } from 'src/routes';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '2em'
  },
  title: 'Search',
  appArea: 'search',
  clearReusableRoutes: true,
  requiredStates: [ ANALYTICS_FILTERING_OPTIONS_STATE_NAME ]
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: ANALYTICS_PATH + '/' + SEARCH_PATH,
        canActivate: [ RbkDatabaseStateGuard ],
        component: SearchPageComponent,
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
    SmzChartModule,
    CardModule
  ],
  exports: [],
  declarations: [
    SearchPageComponent
  ],
  providers: [

  ],
})
export class SearchModule { }