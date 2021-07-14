import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { DashboardFeatureActions } from './dashboard.actions';
import { DiagnosticsService } from '@services/api/diagnostics.service';

export const DIAGNOSTICS_DASHBOARD_FEATURE_STATE_NAME = 'diagnosticsDashboard';

export interface DashboardFeatureStateModel {
  data: any;
}

@State<DashboardFeatureStateModel>({
  name: DIAGNOSTICS_DASHBOARD_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class DashboardFeatureState {

  constructor(private diagnostics: DiagnosticsService) { }

  @Action(DashboardFeatureActions.Filter)
  public filter$(ctx: StateContext<DashboardFeatureStateModel>, action: DashboardFeatureActions.Filter): Observable<any> {
    return this.diagnostics.getDashboardData(action.dateFrom, action.dateTo, action.groupingType).pipe(
      tap((result: any) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
