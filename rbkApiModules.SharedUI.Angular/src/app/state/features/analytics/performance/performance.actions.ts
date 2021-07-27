
export namespace PerformanceFeatureActions {
  export class LoadAll {
    public static readonly type = '[Analytics] Load Performance Data';
    constructor(public endpoint: string, public dateFrom: Date, public dateTo: Date, public groupingType: number) {}
  }
}
