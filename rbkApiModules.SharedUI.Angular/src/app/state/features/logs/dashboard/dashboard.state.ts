import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { DashboardFeatureActions } from './dashboard.actions';
import { LogsService } from '@services/api/logs.service';

export const LOGS_DASHBOARD_FEATURE_STATE_NAME = 'logsDashboard';

export interface DashboardFeatureStateModel {
  data: any;
}

@State<DashboardFeatureStateModel>({
  name: LOGS_DASHBOARD_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class DashboardFeatureState {

  constructor(private logs: LogsService) { }

  @Action(DashboardFeatureActions.Filter)
  public filter$(ctx: StateContext<DashboardFeatureStateModel>, action: DashboardFeatureActions.Filter): Observable<any> {
    return this.logs.getDashboardData(action.dateFrom, action.dateTo, action.groupingType).pipe(
      tap((result: any) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
