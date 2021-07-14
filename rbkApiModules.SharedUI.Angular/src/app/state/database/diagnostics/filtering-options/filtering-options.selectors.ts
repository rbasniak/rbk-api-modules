import { FilteringOptions } from '@models/diagnostics/filtering-options';
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
  public static sources(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.sources.map(x => ({ id: x, name: x === '' || x == null ? 'Without source' : x }));
  }

  @Selector([FilteringOptionsState])
  public static operatingSystems(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.operatingSystems.map(x => ({ id: x, name: x === '' || x == null ? 'Without operating system' : x }));
  }

  @Selector([FilteringOptionsState])
  public static messages(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.messages.map(x => ({ id: x, name: x === '' || x == null ? 'Without message' : x }));
  }

  @Selector([FilteringOptionsState])
  public static layers(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.layers.map(x => ({ id: x, name: x === '' || x == null ? 'Without application layer' : x }));
  }

  @Selector([FilteringOptionsState])
  public static devices(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.devices.map(x => ({ id: x, name: x === '' || x == null ? 'Without device' : x }));
  }

  @Selector([FilteringOptionsState])
  public static browsers(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.browsers.map(x => ({ id: x, name: x === '' || x == null ? 'Without browser' : x }));
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
