import { AnalyticsEntry } from '@models/analytics/analytics-entry';
import { ChartDefinition } from '@models/chart-definition';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { SearchFeatureState, SearchFeatureStateModel } from './search.state';

export class SearchFeatureSelectors {

  @Selector([SearchFeatureState])
  public static results(state: SearchFeatureStateModel): AnalyticsEntry[] {
    return state.data.searchResults.map(x => ({...x,
      responseSize: Math.round(x.responseSize / 1024 * 10) / 10,
      requestSize: Math.round(x.requestSize / 1024 * 10) / 10 }));
  }

  @Selector([SearchFeatureState])
  public static charts(state: SearchFeatureStateModel): ChartDefinition[] {
    return cloneDeep(state.data.charts);
  }
}
