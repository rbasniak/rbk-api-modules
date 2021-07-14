import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AnalyticsService } from '@services/api/analytics.service';
import { DashboardFeatureActions } from './dashboard.actions';

export const ANALYTICS_DASHBOARD_FEATURE_STATE_NAME = 'analyticsDashboard';

export interface DashboardFeatureStateModel {
  data: any;
}

@State<DashboardFeatureStateModel>({
  name: ANALYTICS_DASHBOARD_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class DashboardFeatureState {

  constructor(private analytics: AnalyticsService) { }

  @Action(DashboardFeatureActions.Filter)
  public filter$(ctx: StateContext<DashboardFeatureStateModel>, action: DashboardFeatureActions.Filter): Observable<any> {
    return this.analytics.getDashboardData(action.dateFrom, action.dateTo, action.groupingType).pipe(
      tap((result: any) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
