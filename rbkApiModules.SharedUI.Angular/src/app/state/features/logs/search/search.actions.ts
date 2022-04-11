import { FilterData } from '@models/logs/filter-data';

export namespace SearchFeatureActions {
  export class Search {
    public static readonly type = '[Logs] Search Entries';
    constructor(public data: FilterData) {}
  }
}
