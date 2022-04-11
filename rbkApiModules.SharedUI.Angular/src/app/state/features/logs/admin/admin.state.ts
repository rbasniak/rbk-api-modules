import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { ToastActions } from 'ngx-smz-ui';
import { LogsService } from '@services/api/logs.service';
import { AdminFeatureActions } from './admin.actions';

export const LOGS_ADMIN_FEATURE_STATE_NAME = 'logsAdmin';

export interface AdminFeatureStateModel {
}

@State<AdminFeatureStateModel>({
  name: LOGS_ADMIN_FEATURE_STATE_NAME,
  defaults: { }
})
@Injectable()
export class AdminFeatureState {

  constructor(private logs: LogsService) { }

  @Action(AdminFeatureActions.DeleteOldData)
  public deleteOldData$(ctx: StateContext<AdminFeatureStateModel>, action: AdminFeatureActions.DeleteOldData): Observable<void> {
    return this.logs.deleteOldData(action.daysToKeep).pipe(
      tap(() => {
        ctx.dispatch(new ToastActions.Success('Data successfully deleted.'));
      })
    );
  }
}
