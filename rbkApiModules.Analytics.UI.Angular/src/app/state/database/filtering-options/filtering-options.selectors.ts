import { FilteringOptions } from '@models/filtering-options';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { SimpleNamedEntity } from 'ngx-smz-dialogs';
import { FilteringOptionsState, FilteringOptionsStateModel } from './filtering-options.state';

export class FilteringOptionsSelectors {
  private static noDataOptions = [ { id: null as any, name: 'No data available' } ];

  @Selector([FilteringOptionsState])
  public static all(state: FilteringOptionsStateModel): FilteringOptions {
    return cloneDeep(state.data);
  }

  @Selector([FilteringOptionsState])
  public static versions(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.versions.map(x => ({ id: x, name: x === '' || x == null ? 'Without version' : x }));
  }

  @Selector([FilteringOptionsState])
  public static areas(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.areas.map(x => ({ id: x, name: x === '' || x == null ? 'Without area' : x }));
  }

  @Selector([FilteringOptionsState])
  public static domains(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.domains.map(x => ({ id: x, name: x === '' || x == null ? 'Without domain' : x }));
  }

  @Selector([FilteringOptionsState])
  public static endpoints(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.actions.map(x => ({ id: x, name: x === '' || x == null ? 'Without domain' : x }));
  }

  @Selector([FilteringOptionsState])
  public static responses(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.responses.map(x => ({ id: x, name: x === '' || x == null ? 'Without response' : x }));
  }

  @Selector([FilteringOptionsState])
  public static users(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.users.map(x => ({ id: x, name: x === '' || x == null ? 'Without user' : x }));
  }

  @Selector([FilteringOptionsState])
  public static userAgents(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.agents.map(x => ({ id: x, name: x === '' || x == null ? 'Without user agent' : x }));
  }
}
