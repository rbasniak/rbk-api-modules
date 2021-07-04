import { SmzLayoutsConfig, SmzContentTheme, SmzLoader, HephaestusLayout, MenuType, SidebarState, AthenaLayout } from 'ngx-smz-ui';
import packageInfo from '../../../package.json';

export const smzHephaestusConfig: HephaestusLayout = {
  menu: MenuType.STATIC,
  sidebarState: SidebarState.ACTIVE,
  mobileSidebarState: SidebarState.INACTIVE,
  sidebarWidth: '16rem',
  sidebarSlimWidth: '6rem',
};

export const smzAthenaConfig: AthenaLayout = {
  menu: MenuType.HORIZONTAL,
  sidebarState: SidebarState.INACTIVE,
  mobileSidebarState: SidebarState.INACTIVE,
  sidebarWidth: '16rem',
  sidebarSlimWidth: '6rem',
};

export const smzLayoutsConfig: SmzLayoutsConfig = {
  debugMode: false,
  appLogo: {
    horizontal: {
      dark: 'assets/layout/images/horizontal-dark.svg',
      light: 'assets/layout/images/horizontal-light.svg'
    },
    vertical: {
      dark: 'assets/layout/images/vertical-dark.svg',
      light: 'assets/layout/images/horizontal-light.svg'
    },
    typo: {
      dark: 'assets/layout/images/typo-dark.svg',
      light: 'assets/layout/images/typo-light.svg'
    },
    icon: {
      dark: 'assets/layout/images/icon-dark.svg',
      light: 'assets/layout/images/icon-light.svg'
    },
  },
  appName: 'RBK Shared UI',
  usernameProperty: 'displayName',
  useAvatar: true,
  footer: {
    leftSideText: `(v${packageInfo.version}) RBK Shared UI`,
    rightSideText: '',
    showAppName: true,
    showLogo: true,
  },
  toast: {
    position: 'bottom-right'
  },
  themes: {
    content: SmzContentTheme.PRIMEONE_LIGHT,
    custom: {
      id: 'rbk-shared-ui',
      name: 'RBK Shared UI',
      tone: 'light',
      color: '#ffffff',
      constrast: '#000000',
      schemas: [
        { id: '--primary-color', name: '#000000' },
        { id: '--primary-color-text', name: '#ffffff' },
        { id: '--primary-color-menu-bg', name: '#ffffff' },
        { id: '--primary-color-menu-bg-hover', name: '#EEEEEE' },
        { id: '--primary-color-menu-text', name: '#212121' },
        { id: '--primary-color-menu-text-hover', name: '#000000' },
        { id: '--primary-color-menu-active', name: '#000000' },
        { id: '--primary-color-loading', name: '#F5F5F5' },
        { id: '--primary-color-loading-bg', name: '#000000' }
      ]
    },
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
    isEnabled: false,
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