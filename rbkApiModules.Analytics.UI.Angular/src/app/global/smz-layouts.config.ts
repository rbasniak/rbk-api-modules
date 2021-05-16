import { SmzLayoutsConfig, SmzContentTheme, SmzLoader, HephaestusLayout, MenuType, SidebarState, AthenaLayout, ColorSchemaDefinition } from 'ngx-smz-ui';
export const smzHephaestusConfig: HephaestusLayout = {
  menu: MenuType.STATIC,
  sidebarState: SidebarState.ACTIVE,
  sidebarWidth: '16rem',
  sidebarSlimWidth: '6rem',
};

export const smzAthenaConfig: AthenaLayout = {
  menu: MenuType.HORIZONTAL,
  sidebarState: SidebarState.ACTIVE,
  mobileSidebarState: SidebarState.INACTIVE,
  sidebarWidth: '16rem',
  sidebarSlimWidth: '6rem',
};

export const smzLayoutsConfig: SmzLayoutsConfig = {
  debugMode: false,
  appLogo: {
    horizontal: {
      dark: 'assets/images/logos/horizontal-dark.svg',
      light: 'assets/images/logos/horizontal-light.svg'
    },
    vertical: {
      dark: 'assets/images/logos/vertical-dark.svg',
      light: 'assets/images/logos/vertical-light.svg'
    },
    typo: {
      dark: 'assets/images/logos/typo-dark.svg',
      light: 'assets/images/logos/typo-light.svg'
    },
    icon: {
      dark: 'assets/images/logos/icon-dark.svg',
      light: 'assets/images/logos/icon-light.svg'
    },
  },
  appName: 'RBK Analytics',
  usernameProperty: 'username',
  footer: {

    showAppName: false,
    showLogo: false,
  },
  toast: {
    position: 'bottom-right'
  },
  themes: {
    content: SmzContentTheme.PRIMEONE_LIGHT,
    schema: ColorSchemaDefinition.E_LIBRA
  },
  loader: {
    type: SmzLoader.CUBE,
    title: 'Please wait...',
    message: 'Preparing your data'
  },
  pages: {
    errorTitle: 'Erro',
    errorMessage: 'There was an error while processing your request.',
    errorImagePath: 'assets/images/pages/bg-error.jpg',
    notFoundTitle: 'Page not found',
    notFoundMessage: 'The requested route does not exist.',
    notFoundImagePath: 'assets/images/pages/bg-404.jpg',
  },
  assistance: {
    isEnabled: true,
    sidebarData: {
      position: 'right'
    },
    buttonPosition: 'right-bottom'
  },
  dialogs: {
    closeAllAfterNavigate: true,
  },
  applicationActions: {
    registerLogs: true,
  }
};