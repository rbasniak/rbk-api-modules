import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { LogsService } from '@services/api/logs.service';
import { SearchFeatureActions } from './search.actions';
import { SearchResults } from '@models/logs/search-results';

export const LOGS_SEARCH_FEATURE_STATE_NAME = 'logssSearch';

export interface SearchFeatureStateModel {
  data: SearchResults;
}

@State<SearchFeatureStateModel>({
  name: LOGS_SEARCH_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class SearchFeatureState {

  constructor(private logs: LogsService) { }

  @Action(SearchFeatureActions.Search)
  public search$(ctx: StateContext<SearchFeatureStateModel>, action: SearchFeatureActions.Search): Observable<SearchResults> {
    return this.logs.search(action.data).pipe(
      tap((result: SearchResults) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
