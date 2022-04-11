import { State, Action, StateContext, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { FilteringOptions } from '@models/logs/filtering-options';
import { LogsService } from '@services/api/logs.service';
import { FilteringOptionsActions } from './filtering-options.actions';

export const LOGS_FILTERING_OPTIONS_STATE_NAME = 'logsFilteringOptions';

export interface FilteringOptionsStateModel {
  lastUpdated: Date | null;
  data: FilteringOptions;
}

@State<FilteringOptionsStateModel>({
  name: LOGS_FILTERING_OPTIONS_STATE_NAME,
  defaults: { lastUpdated: null, data: null }
})
@Injectable()
export class FilteringOptionsState {

  constructor(private logs: LogsService, private store: Store) { }

  @Action(FilteringOptionsActions.LoadAll)
  public loadAll$(ctx: StateContext<FilteringOptionsStateModel>): Observable<FilteringOptions> {
    return this.logs.getFilteringOptions().pipe(
      tap((result: FilteringOptions) => {
        ctx.patchState({
          lastUpdated: new Date(),
          data: result,
        });
      })
    );
  }
}
