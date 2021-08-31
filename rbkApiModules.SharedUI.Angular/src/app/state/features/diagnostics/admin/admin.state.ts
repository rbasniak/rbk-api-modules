import { State, Action, StateContext } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { ToastActions } from 'ngx-smz-ui';
import { DiagnosticsService } from '@services/api/diagnostics.service';
import { AdminFeatureActions } from './admin.actions';

export const DIAGNOSTICS_ADMIN_FEATURE_STATE_NAME = 'diagnosticsAdmin';

export interface AdminFeatureStateModel {
}

@State<AdminFeatureStateModel>({
  name: DIAGNOSTICS_ADMIN_FEATURE_STATE_NAME,
  defaults: { }
})
@Injectable()
export class AdminFeatureState {

  constructor(private diagnostics: DiagnosticsService) { }

  @Action(AdminFeatureActions.DeleteOldData)
  public deleteOldData$(ctx: StateContext<AdminFeatureStateModel>, action: AdminFeatureActions.DeleteOldData): Observable<void> {
    return this.diagnostics.deleteOldData(action.daysToKeep).pipe(
      tap(() => {
        ctx.dispatch(new ToastActions.Success('Data successfully deleted.'));
      })
    );
  }
}
