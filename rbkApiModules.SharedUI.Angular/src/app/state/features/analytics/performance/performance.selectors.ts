import { PerformanceData } from '@models/analytics/performance-data';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { PerformanceFeatureState, PerformanceFeatureStateModel } from './performance.state';

export class PerformanceFeatureSelectors {

  @Selector([PerformanceFeatureState])
  public static data(state: PerformanceFeatureStateModel): PerformanceData {
    return cloneDeep(state.data);
  }
}
