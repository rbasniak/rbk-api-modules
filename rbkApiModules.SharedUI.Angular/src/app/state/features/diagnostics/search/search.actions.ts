import { FilterData } from '@models/diagnostics/filter-data';

export namespace SearchFeatureActions {
  export class Search {
    public static readonly type = '[Diagnostics] Search Entries';
    constructor(public data: FilterData) {}
  }
}
