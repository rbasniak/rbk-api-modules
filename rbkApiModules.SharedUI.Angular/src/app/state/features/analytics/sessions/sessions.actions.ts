
export namespace SessionsFeatureActions {
  export class LoadAll {
    public static readonly type = '[Analytics] Load Sessions';
    constructor(public dateFrom: Date, public dateTo: Date, public groupingType: number) {}
  }
}
