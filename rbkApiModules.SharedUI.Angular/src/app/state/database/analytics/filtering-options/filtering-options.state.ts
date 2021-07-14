import { State, Action, StateContext, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { FilteringOptions } from '@models/analytics/filtering-options';
import { AnalyticsService } from '@services/api/analytics.service';
import { FilteringOptionsActions } from './filtering-options.actions';

export const ANALYTICS_FILTERING_OPTIONS_STATE_NAME = 'analyticsFilteringOptions';

export interface FilteringOptionsStateModel {
  lastUpdated: Date | null;
  data: FilteringOptions;
}

@State<FilteringOptionsStateModel>({
  name: ANALYTICS_FILTERING_OPTIONS_STATE_NAME,
  defaults: { lastUpdated: null, data: null }
})
@Injectable()
export class FilteringOptionsState {

  constructor(private analytics: AnalyticsService, private store: Store) { }

  @Action(FilteringOptionsActions.LoadAll)
  public loadAll$(ctx: StateContext<FilteringOptionsStateModel>): Observable<FilteringOptions> {
    return this.analytics.getFilteringOptions().pipe(
      tap((result: FilteringOptions) => {
        ctx.patchState({
          lastUpdated: new Date(),
          data: result,
        });
      })
    );
  }
}
