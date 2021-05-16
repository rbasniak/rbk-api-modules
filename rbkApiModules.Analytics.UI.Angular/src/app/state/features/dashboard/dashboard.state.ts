import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AnalyticsService } from '@services/api/analytics.service';
import { DashboardFeatureActions } from './dashboard.actions';
import { ChartResponse } from '@models/chart-response';

export const DASHBOARD_FEATURE_STATE_NAME = 'DashboardFeature';

export interface DashboardFeatureStateModel {
  data: ChartResponse[];
}

@State<DashboardFeatureStateModel>({
  name: DASHBOARD_FEATURE_STATE_NAME,
  defaults: { data: [] }
})
@Injectable()
export class DashboardFeatureState {

  constructor(private analytics: AnalyticsService) { }

  @Action(DashboardFeatureActions.Filter)
  public filter$(ctx: StateContext<DashboardFeatureStateModel>, action: DashboardFeatureActions.Filter): Observable<ChartResponse[]> {
    return this.analytics.getDashboardData(action.dateFrom, action.dateTo, action.groupingType).pipe(
      tap((result: ChartResponse[]) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
