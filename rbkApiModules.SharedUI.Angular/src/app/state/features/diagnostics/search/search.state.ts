import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { DiagnosticsService } from '@services/api/diagnostics.service';
import { SearchFeatureActions } from './search.actions';
import { SearchResults } from '@models/diagnostics/search-results';

export const DIAGNOSTICS_SEARCH_FEATURE_STATE_NAME = 'diagnosticsSearch';

export interface SearchFeatureStateModel {
  data: SearchResults;
}

@State<SearchFeatureStateModel>({
  name: DIAGNOSTICS_SEARCH_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class SearchFeatureState {

  constructor(private diagnostics: DiagnosticsService) { }

  @Action(SearchFeatureActions.Search)
  public search$(ctx: StateContext<SearchFeatureStateModel>, action: SearchFeatureActions.Search): Observable<SearchResults> {
    return this.diagnostics.search(action.data).pipe(
      tap((result: SearchResults) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
