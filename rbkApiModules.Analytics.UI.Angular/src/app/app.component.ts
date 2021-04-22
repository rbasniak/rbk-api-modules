import { Component, OnInit } from '@angular/core';
import { Store } from '@ngxs/store';
import { BoilerplateService } from 'ngx-rbk-utils';
import { SmzNotification } from 'ngx-smz-ui';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { COURSES_PATH } from 'src/routes';

@Component({
  selector: 'app-root',
  template: '<smz-ui-hephaestus-layout [menu]="menu" [profile]="profile" [notifications]="notifications"><router-outlet></router-outlet></smz-ui-hephaestus-layout>',
})
export class AppComponent implements OnInit {
  public menu: MenuItem[];
  public profile: MenuItem[];
  public notifications: SmzNotification[];

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
        label: 'Analytics',
        icon: 'far fa-list-alt',
        items: [
          { label: 'Dashboard', icon: 'far fa-list-alt', routerLink: [COURSES_PATH] },
          { label: 'Search', icon: 'far fa-list-alt', routerLink: [COURSES_PATH] },
        ]
      }
    ];

    this.profile = [
      {
        label: 'Perfil',
        icon: 'pi-user',
        routerLink: ['/home'],
      },
      {
        label: 'Configurações',
        icon: 'pi-cog',
        routerLink: ['/settings'],
      },
      {
        label: 'Logout',
        icon: 'pi-power-off',
        routerLink: ['/login'],
      },
    ];

    this.notifications = [
      {
        summary: 'Aviso 1',
        details: 'Você tem <strong>3</strong> novas tarefas',
        icon: 'pi-shopping-cart',
      },
      {
        summary: 'Você teve um aumento',
        details: 'Seu salário foi depositado na conta',
        icon: 'pi-check-square',
      }
    ];

  }
}
