
export namespace AdminFeatureActions {
  export class DeleteBasedOnPathText {
    public static readonly type = '[Analytics] Delete Based on Path String';
    constructor(public searchText: string) {}
  }
}
