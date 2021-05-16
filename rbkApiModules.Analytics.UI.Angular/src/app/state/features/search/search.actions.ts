import { FilterData } from '@models/filter-data';

export namespace SearchFeatureActions {
  export class Search {
    public static readonly type = '[Analytics] Search Entries';
    constructor(public data: FilterData) {}
  }
}
