import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AnalyticsService } from '@services/api/analytics.service';
import { SearchFeatureActions } from './search.actions';
import { SearchResults } from '@models/search-results';

export const SEARCH_FEATURE_STATE_NAME = 'SearchFeature';

export interface SearchFeatureStateModel {
  data: SearchResults;
}

@State<SearchFeatureStateModel>({
  name: SEARCH_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class SearchFeatureState {

  constructor(private analytics: AnalyticsService) { }

  @Action(SearchFeatureActions.Search)
  public search$(ctx: StateContext<SearchFeatureStateModel>, action: SearchFeatureActions.Search): Observable<SearchResults> {
    return this.analytics.search(action.data).pipe(
      tap((result: SearchResults) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
