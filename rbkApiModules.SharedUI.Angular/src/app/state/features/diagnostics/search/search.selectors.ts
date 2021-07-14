import { ChartDefinition } from '@models/chart-definition';
import { DiagnosticsEntry } from '@models/diagnostics/diagnostics-entry';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { SearchFeatureState, SearchFeatureStateModel } from './search.state';

export class SearchFeatureSelectors {

  @Selector([SearchFeatureState])
  public static results(state: SearchFeatureStateModel): DiagnosticsEntry[] {
    return state.data.searchResults.map(x => ({...x,
      responseSize: Math.round(x.responseSize / 1024 * 10) / 10,
      requestSize: Math.round(x.requestSize / 1024 * 10) / 10 }));
  }

  @Selector([SearchFeatureState])
  public static charts(state: SearchFeatureStateModel): ChartDefinition[] {
    return cloneDeep(state.data.charts);
  }
}
