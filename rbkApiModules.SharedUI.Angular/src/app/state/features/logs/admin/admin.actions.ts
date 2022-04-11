
export namespace AdminFeatureActions {
  export class DeleteOldData {
    public static readonly type = '[Logs] Delete Old Data';
    constructor(public daysToKeep: number) {}
  }
}
