import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSmzTablesModule, SmzRouteData } from 'ngx-smz-ui';
import { RbkDatabaseStateGuard } from 'ngx-rbk-utils';
import { CoursesComponent } from './courses.component';
import { NgxSmzFormsModule } from 'ngx-smz-dialogs';
import { ButtonModule } from 'primeng/button';
import { UI_DEFINITIONS_STATE_NAME } from 'ngx-rbk-utils';
import { COURSES_STATE_NAME } from '@state/database/courses/courses.state';

const data: SmzRouteData = {
  layout: {
    mode: 'layout',
    contentPadding: '2em'
  },
  title: 'Cursos',
  appArea: 'courses',
  clearReusableRoutes: true,
  requiredStates: [ COURSES_STATE_NAME, UI_DEFINITIONS_STATE_NAME ]
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
        canActivate: [ RbkDatabaseStateGuard ],
        component: CoursesComponent,
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
    NgxSmzTablesModule
  ],
  exports: [],
  declarations: [
    CoursesComponent
  ],
  providers: [

  ],
})
export class CoursesModule { }