export interface MenuData {
  useAnalytics: boolean;
  useDiagnostics: boolean;
  customRoutes: CustomRoute[];
}

export interface CustomRoute {
  route: string;
  name: string;
  icon: string;
}