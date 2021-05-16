import { environment } from '@environments/environment';
import { FilteringOptionsActions } from '@state/database/filtering-options/filtering-options.actions';
import { FilteringOptionsState, FILTERING_OPTIONS_STATE_NAME } from '@state/database/filtering-options/filtering-options.state';
import { SearchFeatureState, SEARCH_FEATURE_STATE_NAME } from '@state/features/search/search.state';
import { NgxRbkUtilsConfig } from 'ngx-rbk-utils';
import { ERROR_PAGE_PATH, HOME_PATH, LOGIN_PATH } from 'src/routes';

// Database

// Features

// Actions

export const rbkConfig: NgxRbkUtilsConfig = {
  debugMode: false,
  applicationName: 'TMP',
  useTitleService: true,
  routes: {
    nonAuthenticatedRoot: `/${LOGIN_PATH}`,
    authenticatedRoot: `/${HOME_PATH}`,
    login: `/${LOGIN_PATH}`,
    error: `/${ERROR_PAGE_PATH}`
  },
  diagnostics: {
    url: `${environment.serverUrl}/api/diagnostics`,
  },
  uiDefinitions: {
    url: `${environment.serverUrl}/api/ui-definitions`,
    httpBehavior: {
      authentication: false,
      compression: true,
      errorHandlingType: 'dialog',
      loadingBehavior: 'global',
      needToRefreshToken: false,
    }
  },
  authentication: {
    localStoragePrefix: 'TMP',
    login: {
      url: `${environment.serverUrl}/api/auth/login`,
      errorHandlingType: 'toast',
      responsePropertyName: 'accessToken',
      loadingBehavior: 'global',
    },
    refreshToken: {
      url: `${environment.serverUrl}/api/auth/refresh-token`,
      errorHandlingType: 'toast',
      responsePropertyName: 'refreshToken',
      loadingBehavior: 'global',
    },
    accessTokenClaims: [
    ]
  },
  state: {
    database: {
      [FILTERING_OPTIONS_STATE_NAME]: {
        state: FilteringOptionsState,
        cacheTimeout: 999,
        loadAction: FilteringOptionsActions.LoadAll,
        clearFunction: (): any => ({ data: null, lastUpdated: null })
      }
    },
    feature: {
      [SEARCH_FEATURE_STATE_NAME]: {
        state: SearchFeatureState,
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