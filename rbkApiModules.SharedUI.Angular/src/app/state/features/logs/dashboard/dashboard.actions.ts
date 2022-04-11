
export namespace DashboardFeatureActions {
  export class Filter {
    public static readonly type = '[Logs] Filter Dashboard';
    constructor(public dateFrom: Date, public dateTo: Date, public groupingType: number) {}
  }
}
