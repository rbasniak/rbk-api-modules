import { Component, OnInit } from '@angular/core';
import { Select } from '@ngxs/store';
import { MenuSelectors } from '@state/database/menu/menu.selectors';
import { BoilerplateService } from 'ngx-rbk-utils';
import { ThemeManagerService } from 'ngx-smz-ui';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { Observable } from 'rxjs';
import {ResizeObserver as ResizeObserverPolyfill} from '@juggle/resize-observer';

@Component({
  selector: 'app-root',
  template: '<smz-ui-hephaestus-layout [menu]="menu$ | async"><router-outlet></router-outlet></smz-ui-hephaestus-layout>',
})
export class AppComponent implements OnInit {
  @Select(MenuSelectors.all) public menu$: Observable<MenuItem[]>;

  constructor(private primengConfig: PrimeNGConfig, private boilerplateService: BoilerplateService, private themeManager: ThemeManagerService) {
    this.boilerplateService.init();
    this.themeManager.createCss('assets/styles/overrides.css');
  }

  public ngOnInit(): void {
    this.primengConfig.ripple = true;

    if (typeof window !== 'undefined') {
      window.ResizeObserver = window.ResizeObserver || ResizeObserverPolyfill;
    }
  }
}
