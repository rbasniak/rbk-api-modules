import { Component, OnInit } from '@angular/core';
import { Store } from '@ngxs/store';
import { BoilerplateService } from 'ngx-rbk-utils';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { DASHBOARD_PAGE_ROUTE, SEARCH_PAGE_ROUTE } from 'src/routes';

@Component({
  selector: 'app-root',
  template: '<smz-ui-hephaestus-layout [menu]="menu"><router-outlet></router-outlet></smz-ui-hephaestus-layout>',
})
export class AppComponent implements OnInit {
  public menu: MenuItem[];

  constructor(private primengConfig: PrimeNGConfig, private boilerplateService: BoilerplateService, private store: Store) {
    this.boilerplateService.init();

    this.setupMenu();
  }

  public ngOnInit(): void {
    this.primengConfig.ripple = true;
  }

  public setupMenu(): void {
    this.menu = [
      {
        label: 'Search',
        items: [
          { label: 'Search', icon: 'fas fa-search', routerLink: SEARCH_PAGE_ROUTE },
        ]
      },
      {
        label: 'Statistics',
        icon: 'far fa-list-alt',
        items: [
          { label: 'Dashboard', icon: 'fas fa-chart-line', routerLink: DASHBOARD_PAGE_ROUTE },
          { label: 'Performance', icon: 'fas fa-tachometer-alt', routerLink: [] },
        ]
      }
    ];
  }
}
