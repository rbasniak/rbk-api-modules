import { ApplicationStateModel, AppStateModel, FeaturesState } from 'ngx-rbk-utils';
import { DatabaseState } from './database/database.state';

export interface RootStateModel extends AppStateModel {
  database: DatabaseState;
  features: FeaturesState;
  application: ApplicationStateModel;
}