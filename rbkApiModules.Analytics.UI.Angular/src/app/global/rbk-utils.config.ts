import { DatabaseStoreStateModel, getInitialDatabaseStoreState, NgxRbkUtilsConfig } from 'ngx-rbk-utils';
import { environment } from '@environments/environment';
import { ERROR_PAGE_PATH, HOME_PATH, LOGIN_PAGE_PATH } from 'src/routes';

// Database

// Features

// Actions

export const rbkConfig: NgxRbkUtilsConfig = {
  debugMode: false,
  applicationName: 'TMP',
  useTitleService: true,
  routes: {
    nonAuthenticatedRoot: `/${LOGIN_PAGE_PATH}`,
    authenticatedRoot: `/${HOME_PATH}`,
    login: `/${LOGIN_PAGE_PATH}`,
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
      { claimName: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name', propertyName: 'username', type: 'string' },
      { claimName: 'rol', propertyName: 'roles', type: 'array' },
      { claimName: 'avatar', propertyName: 'avatar', type: 'string' },
    ]
  },
  state: {
    database: {
      // [COURSES_STATE_NAME]: {
      //   state: CoursesState,
      //   cacheTimeout: 0,
      //   loadAction: CoursesActions.LoadAll,
      //   clearFunction: (): DatabaseStoreStateModel<CoursesDetails> => getInitialDatabaseStoreState<CoursesDetails>()
      // }
    },
    feature: {
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