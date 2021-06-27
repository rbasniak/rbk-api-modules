import { ChartDefinition } from '@models/chart-definition';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { DashboardFeatureState, DashboardFeatureStateModel } from './dashboard.state';

export class DashboardFeatureSelectors {

  @Selector([DashboardFeatureState])
  public static data(state: DashboardFeatureStateModel): ChartDefinition[] {
    return cloneDeep(state.data);
  }
}
