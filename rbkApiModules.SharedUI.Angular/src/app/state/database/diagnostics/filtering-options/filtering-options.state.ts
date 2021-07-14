import { State, Action, StateContext, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { FilteringOptions } from '@models/diagnostics/filtering-options';
import { DiagnosticsService } from '@services/api/diagnostics.service';
import { FilteringOptionsActions } from './filtering-options.actions';

export const DIAGNOSTICS_FILTERING_OPTIONS_STATE_NAME = 'diagnosticsFilteringOptions';

export interface FilteringOptionsStateModel {
  lastUpdated: Date | null;
  data: FilteringOptions;
}

@State<FilteringOptionsStateModel>({
  name: DIAGNOSTICS_FILTERING_OPTIONS_STATE_NAME,
  defaults: { lastUpdated: null, data: null }
})
@Injectable()
export class FilteringOptionsState {

  constructor(private diagnostics: DiagnosticsService, private store: Store) { }

  @Action(FilteringOptionsActions.LoadAll)
  public loadAll$(ctx: StateContext<FilteringOptionsStateModel>): Observable<FilteringOptions> {
    return this.diagnostics.getFilteringOptions().pipe(
      tap((result: FilteringOptions) => {
        ctx.patchState({
          lastUpdated: new Date(),
          data: result,
        });
      })
    );
  }
}
