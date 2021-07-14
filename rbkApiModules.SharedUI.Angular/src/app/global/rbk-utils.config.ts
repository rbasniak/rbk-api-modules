import { environment } from '@environments/environment';
import { FilteringOptionsActions as AnalyticsFilteringOptionsActions } from '@state/database/analytics/filtering-options/filtering-options.actions';
import { FilteringOptionsState as AnalyticsFilteringOptionsState, ANALYTICS_FILTERING_OPTIONS_STATE_NAME } from '@state/database/analytics/filtering-options/filtering-options.state';
import { DashboardFeatureState as AnalyticsDashboardFeatureState, ANALYTICS_DASHBOARD_FEATURE_STATE_NAME } from '@state/features/analytics/dashboard/dashboard.state';
import { SearchFeatureState as AnalyticsSearchFeatureState, ANALYTICS_SEARCH_FEATURE_STATE_NAME } from '@state/features/analytics/search/search.state';
import { MenuActions } from '@state/database/menu/menu.actions';
import { MenuState, MENU_STATE_NAME } from '@state/database/menu/menu.state';
import { NgxRbkUtilsConfig } from 'ngx-rbk-utils';
import { ERROR_PAGE_PATH, HOME_PATH, LOGIN_PATH } from 'src/routes';
import { FilteringOptionsActions as DiagnosticsFilteringOptionsActions } from '@state/database/diagnostics/filtering-options/filtering-options.actions';
import { FilteringOptionsState as DiagnosticsFilteringOptionsState, DIAGNOSTICS_FILTERING_OPTIONS_STATE_NAME } from '@state/database/diagnostics/filtering-options/filtering-options.state';
import { DashboardFeatureState as DiagnosticsDashboardFeatureState, DIAGNOSTICS_DASHBOARD_FEATURE_STATE_NAME } from '@state/features/diagnostics/dashboard/dashboard.state';
import { SearchFeatureState as DiagnosticsSearchFeatureState, DIAGNOSTICS_SEARCH_FEATURE_STATE_NAME } from '@state/features/diagnostics/search/search.state';

// Database

// Features

// Actions

export const rbkConfig: NgxRbkUtilsConfig = {
  debugMode: false,
  applicationName: 'RBK',
  useTitleService: true,
  routes: {
    nonAuthenticatedRoot: `/${LOGIN_PATH}`,
    authenticatedRoot: `/${HOME_PATH}`,
    login: `/${LOGIN_PATH}`,
    error: `/${ERROR_PAGE_PATH}`
  },
  diagnostics: {
    url: null,
  },
  uiDefinitions: {
    url: null,
    httpBehavior: {
      authentication: false,
      compression: true,
      errorHandlingType: 'dialog',
      loadingBehavior: 'global',
      needToRefreshToken: false,
    }
  },
  authentication: {
    localStoragePrefix: 'RBK',
    login: {
      url: environment.production ? `${window.location.origin}/api/shared-ui/auth` : `${environment.serverUrl}/api/shared-ui/auth`,
      errorHandlingType: 'toast',
      responsePropertyName: 'accessToken',
      loadingBehavior: 'local',
    },
    refreshToken: {
      url: environment.production ? `${window.location.origin}/api/shared-ui/refresh-token` : `${environment.serverUrl}/api/shared-ui/refresh-token`,
      errorHandlingType: 'toast',
      responsePropertyName: 'refreshToken',
      loadingBehavior: 'global',
    },
    accessTokenClaims: [
    ]
  },
  state: {
    database: {
      [ANALYTICS_FILTERING_OPTIONS_STATE_NAME]: {
        state: AnalyticsFilteringOptionsState,
        cacheTimeout: 999,
        loadAction: AnalyticsFilteringOptionsActions.LoadAll,
        clearFunction: (): any => ({ data: null, lastUpdated: null })
      },
      [DIAGNOSTICS_FILTERING_OPTIONS_STATE_NAME]: {
        state: DiagnosticsFilteringOptionsState,
        cacheTimeout: 999,
        loadAction: DiagnosticsFilteringOptionsActions.LoadAll,
        clearFunction: (): any => ({ data: null, lastUpdated: null })
      },
      [MENU_STATE_NAME]: {
        state: MenuState,
        cacheTimeout: 999,
        loadAction: MenuActions.LoadAll,
        clearFunction: (): any => ({ data: null, lastUpdated: null })
      }
    },
    feature: {
      [ANALYTICS_SEARCH_FEATURE_STATE_NAME]: {
        state: AnalyticsSearchFeatureState,
        clearFunction: (): any => ({ results: [] })
      },
      [ANALYTICS_DASHBOARD_FEATURE_STATE_NAME]: {
        state: AnalyticsDashboardFeatureState,
        clearFunction: (): any => ({ results: [] })
      },
      [DIAGNOSTICS_SEARCH_FEATURE_STATE_NAME]: {
        state: DiagnosticsSearchFeatureState,
        clearFunction: (): any => ({ results: [] })
      },
      [DIAGNOSTICS_DASHBOARD_FEATURE_STATE_NAME]: {
        state: DiagnosticsDashboardFeatureState,
        clearFunction: (): any => ({ results: [] })
      }
    }
  },
  httpBehaviors: {
    defaultParameters: {
      compression: true,
      authentication: true,
      needToRefreshToken: true,
      loadingBehavior: 'global',
      errorHandlingType: 'toast',
      localLoadingTag: null,
      restoreStateOnError: true,
      ignoreErrorHandling: false
    },
  },
  toastConfig: {
    severity: 'success',
    life: 5000,
    sticky: false,
    closable: true,
    successTitle: 'SUCESSO',
    errorTitle: 'ERRO',
    warningTitle: 'AVISO',
    infoTitle: 'INFORMAÇÃO',
  },
  dialogsConfig: {
    errorDialogTitle: 'ERRO',
    warningDialogTitle: 'ALERTA'
  }
};