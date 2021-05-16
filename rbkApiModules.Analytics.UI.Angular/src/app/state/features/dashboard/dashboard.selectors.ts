import { ChartResponse } from '@models/chart-response';
import { Selector } from '@ngxs/store';
import { DashboardFeatureState, DashboardFeatureStateModel } from './dashboard.state';

export class DashboardFeatureSelectors {

  @Selector([DashboardFeatureState])
  public static data(state: DashboardFeatureStateModel): ChartResponse[] {
    return state.data;
  }
}
