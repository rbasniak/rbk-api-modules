import { environment } from '@environments/environment';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { MenuItem } from 'primeng/api';
import { ANALYTICS_DASHBOARD_PAGE_ROUTE, ANALYTICS_SEARCH_PAGE_ROUTE, DIAGNOSTICS_SEARCH_PAGE_ROUTE,
  DIAGNOSTICS_DASHBOARD_PAGE_ROUTE, ANALYTICS_PERFORMANCE_PAGE_ROUTE, ANALYTICS_ADMIN_PAGE_ROUTE,
  DIAGNOSTICS_ADMIN_PAGE_ROUTE, ANALYTICS_SESSIONS_PAGE_ROUTE} from 'src/routes';
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
          { label: 'Search', icon: 'fas fa-search', routerLink: ANALYTICS_SEARCH_PAGE_ROUTE },
          { label: 'Overview', icon: 'fas fa-chart-line', routerLink: ANALYTICS_DASHBOARD_PAGE_ROUTE },
          { label: 'Sessions', icon: 'fas fa-user-clock', routerLink: ANALYTICS_SESSIONS_PAGE_ROUTE },
          { label: 'Performance', icon: 'fas fa-tachometer-alt', routerLink: ANALYTICS_PERFORMANCE_PAGE_ROUTE },
          { label: 'Admin', icon: 'fas fa-shield-alt', routerLink: ANALYTICS_ADMIN_PAGE_ROUTE },
        ]
      });
    }

    if (state.data.useDiagnostics) {
      menu.push({
        label: 'Diagnostics',
        icon: 'far fa-list-alt',
        items: [
          { label: 'Search', icon: 'fas fa-bug', routerLink: DIAGNOSTICS_SEARCH_PAGE_ROUTE },
          { label: 'Overview', icon: 'fas fa-chart-line', routerLink: DIAGNOSTICS_DASHBOARD_PAGE_ROUTE },
          { label: 'Admin', icon: 'fas fa-shield-alt', routerLink: DIAGNOSTICS_ADMIN_PAGE_ROUTE },
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
          localStorage.clear();
          console.log('logout');
        } },
      ]
    });

    return cloneDeep(menu);
  }
}
