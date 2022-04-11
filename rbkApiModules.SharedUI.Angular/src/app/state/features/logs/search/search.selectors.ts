import { ChartDefinition } from '@models/chart-definition';
import { LogsEntry } from '@models/logs/logs-entry';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { SearchFeatureState, SearchFeatureStateModel } from './search.state';

export class SearchFeatureSelectors {

  @Selector([SearchFeatureState])
  public static results(state: SearchFeatureStateModel): LogsEntry[] {
    return state.data.searchResults;
  }

  @Selector([SearchFeatureState])
  public static charts(state: SearchFeatureStateModel): ChartDefinition[] {
    return cloneDeep(state.data.charts);
  }
}
