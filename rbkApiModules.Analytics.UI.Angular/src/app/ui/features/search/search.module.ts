import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSmzTablesModule, SmzChartModule, SmzRouteData } from 'ngx-smz-ui';
import { RbkDatabaseStateGuard } from 'ngx-rbk-utils';
import { NgxSmzFormsModule } from 'ngx-smz-dialogs';
import { ButtonModule } from 'primeng/button';
import { UI_DEFINITIONS_STATE_NAME } from 'ngx-rbk-utils';
import { SearchPageComponent } from './page/search-page.component';
import { FILTERING_OPTIONS_STATE_NAME } from '@state/database/filtering-options/filtering-options.state';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '2em'
  },
  title: 'Search',
  appArea: 'search',
  clearReusableRoutes: true,
  requiredStates: [ FILTERING_OPTIONS_STATE_NAME, UI_DEFINITIONS_STATE_NAME ]
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
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