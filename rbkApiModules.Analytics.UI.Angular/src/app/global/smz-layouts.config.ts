import { SmzLayoutsConfig, SmzContentTheme, SmzLoader, HephaestusLayout, MenuType, SidebarState, AthenaLayout, ColorSchemaDefinition } from 'ngx-smz-ui';
import { version } from '../../../package.json';
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
  appName: 'Web Template Application',
  usernameProperty: 'username',
  footer: {
    leftSideText: `(v${version}) Tecgraf PUC-Rio | Petrobras`,
    rightSideImages: ['assets/images/logos/TecgrafAzulHorizontal.svg', 'assets/images/logos/Principal_h_cor_RGB-no-margin.svg'],
    rightSideText: '',
    showAppName: true,
    showLogo: true,
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
    title: 'Carregando...',
    message: 'Aguarde por favor'
  },
  pages: {
    errorTitle: 'Erro',
    errorMessage: 'Ocorreu um erro com a sua solicitação. Caso persista, entre em contato com seu administrador de sistema.',
    errorImagePath: 'assets/images/pages/bg-error.jpg',
    notFoundTitle: 'Página não encontrada',
    notFoundMessage: 'A rota solicitada não existe ou não se encontra disponível no momento.',
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