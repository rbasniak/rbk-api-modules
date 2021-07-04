import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { SmzRouteData } from 'ngx-smz-ui';
import { ButtonModule } from 'primeng/button';
import { MarkdownPipe } from './components/markdown.pipe';

const data: SmzRouteData = {
  layout: {
    mode: 'menu-only',
    contentPadding: '2em'
  },
  title: 'Home',
  appArea: 'home',
  clearReusableRoutes: true,
  requiredStates: []
};

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
        component: HomeComponent,
        data
      },
    ]
  },
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ButtonModule
  ],
  exports: [],
  declarations: [
    HomeComponent,
    MarkdownPipe
  ],
  providers: [

  ],
})
export class HomeModule { }