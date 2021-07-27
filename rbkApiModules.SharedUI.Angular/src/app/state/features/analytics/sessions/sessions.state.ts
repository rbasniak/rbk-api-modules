import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AnalyticsService } from '@services/api/analytics.service';
import { SessionsFeatureActions } from './sessions.actions';
import { SessionsData } from '@models/analytics/sessions-data';

export const ANALYTICS_SESSIONS_FEATURE_STATE_NAME = 'sessionsDashboard';

export interface SessionsFeatureStateModel {
  data: SessionsData;
}

@State<SessionsFeatureStateModel>({
  name: ANALYTICS_SESSIONS_FEATURE_STATE_NAME,
  defaults: { data: null }
})
@Injectable()
export class SessionsFeatureState {

  constructor(private analytics: AnalyticsService) { }

  @Action(SessionsFeatureActions.LoadAll)
  public filter$(ctx: StateContext<SessionsFeatureStateModel>, action: SessionsFeatureActions.LoadAll): Observable<SessionsData> {
    return this.analytics.getSessionsData(action.dateFrom, action.dateTo, action.groupingType).pipe(
      tap((result: SessionsData) => {
        ctx.patchState({
          data: result,
        });
      })
    );
  }
}
