import { SessionsData } from '@models/analytics/sessions-data';
import { Selector } from '@ngxs/store';
import { cloneDeep } from 'lodash-es';
import { SessionsFeatureState, SessionsFeatureStateModel } from './sessions.state';

export class SessionsFeatureSelectors {

  @Selector([SessionsFeatureState])
  public static data(state: SessionsFeatureStateModel): SessionsData {
    return cloneDeep(state.data);
  }
}
