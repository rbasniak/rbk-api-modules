import { AnalyticsEntry } from '@models/analytics-entry';
import { Selector } from '@ngxs/store';
import { SearchFeatureState, SearchFeatureStateModel } from './search.state';

export class SearchFeatureSelectors {

  @Selector([SearchFeatureState])
  public static results(state: SearchFeatureStateModel): AnalyticsEntry[] {
    return state.results;
  }
}
