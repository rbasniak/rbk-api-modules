import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AnalyticsService } from '@services/api/analytics.service';
import { PerformanceFeatureActions } from './performance.actions';
import { PerformanceData } from '@models/analytics/performance-data';

export const ANALYTICS_PERFORMANCE_FEATURE_STATE_NAME = 'performance';

export interface PerformanceFeatureStateModel {
  data: PerformanceData;
}

@State<PerformanceFeatureStateModel>({
  name: ANALYTICS_PERFORMANCE_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class PerformanceFeatureState {

  constructor(private analytics: AnalyticsService) { }

  @Action(PerformanceFeatureActions.LoadAll)
  public filter$(ctx: StateContext<PerformanceFeatureStateModel>, action: PerformanceFeatureActions.LoadAll): Observable<PerformanceData> {
    return this.analytics.getPerformanceData(action.endpoint, action.dateFrom, action.dateTo, action.groupingType).pipe(
      tap((result: PerformanceData) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
