import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AnalyticsService } from '@services/api/analytics.service';
import { SearchFeatureActions } from './search.actions';
import { AnalyticsEntry } from '@models/analytics-entry';

export const SEARCH_FEATURE_STATE_NAME = 'SearchFeature';

export interface SearchFeatureStateModel {
  results: AnalyticsEntry[];
}

@State<SearchFeatureStateModel>({
  name: SEARCH_FEATURE_STATE_NAME,
  defaults: { results: null }
})
@Injectable()
export class SearchFeatureState {

  constructor(private analytics: AnalyticsService) { }

  @Action(SearchFeatureActions.Search)
  public search$(ctx: StateContext<SearchFeatureStateModel>, action: SearchFeatureActions.Search): Observable<AnalyticsEntry[]> {
    return this.analytics.search(action.data).pipe(
      tap((result: AnalyticsEntry[]) => {
        ctx.patchState({
          results: result,
        });
      })
    );
  }
}
