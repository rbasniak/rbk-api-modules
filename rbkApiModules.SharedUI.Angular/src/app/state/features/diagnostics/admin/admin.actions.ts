
export namespace AdminFeatureActions {
  export class DeleteOldData {
    public static readonly type = '[Diagnostics] Delete Old Data';
    constructor(public daysToKeep: number) {}
  }
}
