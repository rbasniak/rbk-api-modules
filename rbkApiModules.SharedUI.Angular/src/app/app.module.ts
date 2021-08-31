import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LOCALE_ID, NgModule } from '@angular/core';

import { AppRoutingModule } from './app.routing';
import { AppComponent } from './app.component';
import { NgxsModule, Store } from '@ngxs/store';
import { ApplicationActions, buildState, NgxRbkUtilsConfig, NgxRbkUtilsModule } from 'ngx-smz-ui';
import { environment } from '@environments/environment';
import { NgxSmzDialogsModule } from 'ngx-smz-ui';
import { HttpClientModule } from '@angular/common/http';
import { NgxsRouterPluginModule } from '@ngxs/router-plugin';
import { rbkConfig } from './global/rbk-utils.config';
import { smzDialogsConfig } from './global/smz-dialogs.config';
import { smzHephaestusConfig, smzLayoutsConfig } from './global/smz-layouts.config';
import { NgxsReduxDevtoolsPluginModule } from '@ngxs/devtools-plugin';
import { CommonModule, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { ToastModule } from 'primeng/toast';
import { HephaestusLayoutModule, NgxSmzLayoutsModule } from 'ngx-smz-ui';
import { NgPipesModule } from 'ngx-pipes';

registerLocaleData(localePt , 'pt-BR');

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    NgxSmzDialogsModule.forRoot(smzDialogsConfig),
    NgxRbkUtilsModule.forRoot(rbkConfig),
    NgxsModule.forRoot(buildState(), { developmentMode: !environment.production }),
    NgxsRouterPluginModule.forRoot(),
    NgxsReduxDevtoolsPluginModule.forRoot(),
    NgxSmzLayoutsModule.forRoot(smzLayoutsConfig),
    HephaestusLayoutModule.forRoot(smzHephaestusConfig),
    ToastModule,
    NgPipesModule
  ],
  providers: [
    { provide: LOCALE_ID, useFactory: (): string => 'pt-BR' },
    { provide: NgxRbkUtilsConfig, useValue: rbkConfig },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private store: Store) {
    this.store.dispatch(new ApplicationActions.NgRxInitialized());
  }
}