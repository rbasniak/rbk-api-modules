import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { AnalyticsService } from '@services/api/analytics.service';
import { AdminFeatureActions } from './admin.actions';
import { ToastActions } from 'ngx-smz-ui';

export const ANALYTICS_ADMIN_FEATURE_STATE_NAME = 'analyticsAdmin';

export interface AdminFeatureStateModel {
}

@State<AdminFeatureStateModel>({
  name: ANALYTICS_ADMIN_FEATURE_STATE_NAME,
  defaults: { }
})
@Injectable()
export class AdminFeatureState {

  constructor(private analytics: AnalyticsService) { }

  @Action(AdminFeatureActions.DeleteBasedOnPathText)
  public deleteBasedOnPathText$(ctx: StateContext<AdminFeatureStateModel>, action: AdminFeatureActions.DeleteBasedOnPathText): Observable<void> {
    return this.analytics.deleteBasedOnPathText(action.searchText).pipe(
      tap(() => {
        ctx.dispatch(new ToastActions.Success('Data successfully deleted.'));
      })
    );
  }
}
