import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSmzTablesModule, SmzChartModule, SmzRouteData } from 'ngx-smz-ui';
import { RbkDatabaseStateGuard } from 'ngx-smz-ui';
import { NgxSmzFormsModule } from 'ngx-smz-ui';
import { ButtonModule } from 'primeng/button';
import { SearchPageComponent } from './page/search-page.component';
import { DIAGNOSTICS_FILTERING_OPTIONS_STATE_NAME } from '@state/database/diagnostics/filtering-options/filtering-options.state';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { DIAGNOSTICS_PATH, SEARCH_PATH } from 'src/routes';
import { FormatHtmlPipe } from './pipes/format-html.pipe';
import { NgPipesModule } from 'ngx-pipes';
import { EmptyGuardPipe } from './pipes/empty-guard.pipe';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '2em'
  },
  title: 'Search',
  appArea: 'search',
  clearReusableRoutes: true,
  requiredStates: [ DIAGNOSTICS_FILTERING_OPTIONS_STATE_NAME ]
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: DIAGNOSTICS_PATH + '/' + SEARCH_PATH,
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
    CardModule,
    NgPipesModule
  ],
  exports: [],
  declarations: [
    SearchPageComponent,
    FormatHtmlPipe,
    EmptyGuardPipe
  ],
  providers: [

  ],
})
export class SearchModule { }