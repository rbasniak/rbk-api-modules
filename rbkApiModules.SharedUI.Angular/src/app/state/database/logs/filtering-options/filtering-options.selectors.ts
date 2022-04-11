import { FilteringOptions } from '@models/logs/filtering-options';
import { LogLevel } from '@models/logs/logs-entry';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { SimpleEntity, SimpleNamedEntity } from 'ngx-smz-ui';
import { FilteringOptionsState, FilteringOptionsStateModel } from './filtering-options.state';

export class FilteringOptionsSelectors {

  private static noDataOptions = [ { id: null as any, name: 'No data available' } ];

  @Selector([FilteringOptionsState])
  public static all(state: FilteringOptionsStateModel): FilteringOptions {
    return cloneDeep(state.data);
  }

  @Selector([FilteringOptionsState])
  public static messages(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.messages.map(x => ({ id: x, name: x === '' || x == null ? 'Without message' : x }));
  }

  @Selector([FilteringOptionsState])
  public static levels(state: FilteringOptionsStateModel): SimpleEntity<LogLevel>[] {
    if (state.lastUpdated == null) return this.noDataOptions;
    return state.data.levels.map(x => {
      if (x === LogLevel.Warning) {
        return { id:x, name: 'Warning' };
      }
      else if (x === LogLevel.Error) {
        return { id:x, name: 'Error' };
      }
      else if (x === LogLevel.Debug) {
        return { id:x, name: 'Debug' };
      }
      else {
        return { id:-1, name: 'Without level' };
      }
    });
  }

  @Selector([FilteringOptionsState])
  public static layers(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.layers.map(x => ({ id: x, name: x === '' || x == null ? 'Without application layer' : x }));
  }

  @Selector([FilteringOptionsState])
  public static areas(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.areas.map(x => ({ id: x, name: x === '' || x == null ? 'Without area' : x }));
  }

  @Selector([FilteringOptionsState])
  public static versions(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.versions.map(x => ({ id: x, name: x === '' || x == null ? 'Without version' : x }));
  }

  @Selector([FilteringOptionsState])
  public static sources(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.sources.map(x => ({ id: x, name: x === '' || x == null ? 'Without source' : x }));
  }

  @Selector([FilteringOptionsState])
  public static enviroments(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.enviroments.map(x => ({ id: x, name: x === '' || x == null ? 'Without enviroment' : x }));
  }

  @Selector([FilteringOptionsState])
  public static enviromentsVersions(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.enviromentsVersions.map(x => ({ id: x, name: x === '' || x == null ? 'Without enviroment version' : x }));
  }

  @Selector([FilteringOptionsState])
  public static users(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.users.map(x => ({ id: x, name: x === '' || x == null ? 'Without user' : x }));
  }

  @Selector([FilteringOptionsState])
  public static domains(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.domains.map(x => ({ id: x, name: x === '' || x == null ? 'Without domain' : x }));
  }

  @Selector([FilteringOptionsState])
  public static machines(state: FilteringOptionsStateModel): SimpleNamedEntity[] {
    if (state.lastUpdated == null) return this.noDataOptions;

    return state.data.machines.map(x => ({ id: x, name: x === '' || x == null ? 'Without machine name' : x }));
  }
}
