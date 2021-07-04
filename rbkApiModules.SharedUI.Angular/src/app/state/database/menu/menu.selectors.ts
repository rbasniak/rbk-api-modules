import { environment } from '@environments/environment';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { MenuItem } from 'primeng/api';
import { DASHBOARD_PAGE_ROUTE, HOME_PAGE_ROUTE, SEARCH_PAGE_ROUTE } from 'src/routes';
import { MenuState, MenuStateModel } from './menu.state';

export class MenuSelectors {
  @Selector([MenuState])
  public static all(state: MenuStateModel): MenuItem[] {
    const menu = [];

    if (state.data.useAnalytics) {
      menu.push({
        label: 'Statistics',
        icon: 'far fa-list-alt',
        items: [
          { label: 'Search', icon: 'fas fa-search', routerLink: SEARCH_PAGE_ROUTE },
          { label: 'Dashboard', icon: 'fas fa-chart-line', routerLink: DASHBOARD_PAGE_ROUTE },
          { label: 'Performance', icon: 'fas fa-tachometer-alt', routerLink: [] },
        ]
      });
    }

    if (state.data.useDiagnostics) {
      menu.push({
        label: 'Diagnostics',
        icon: 'far fa-list-alt',
        items: [
          { label: 'Trace', icon: 'fas fa-search', routerLink: SEARCH_PAGE_ROUTE },
          { label: 'Dashboard', icon: 'fas fa-chart-line', routerLink: DASHBOARD_PAGE_ROUTE },
        ]
      });
    }

    if (state.data.customRoutes.length > 0) {
      let url = window.location.origin;

      if (!environment.production) {
        url = `${environment.serverUrl}`;
      }

      const customRoutes = {
        label: 'Custom routes',
        icon: 'fas fa-home',
        items: [] as MenuItem[]
      };
      state.data.customRoutes.forEach(route => {
        customRoutes.items.push(
          {
            label: route.name,
            icon: route.icon,
            command: () => window.open(url + route.route, '_blank')
          }
        );
      });

      menu.push(customRoutes);
    }

    menu.push({
      label: 'MISC',
      icon: 'far fa-list-alt',
      items: [
        { label: 'Logout', icon: 'fas fa-sign-out-alt', command: (): void => {
          // TODO: como acessar a store aqui?
          console.log('logout');
        } },
      ]
    });

    return cloneDeep(menu);
  }
}
