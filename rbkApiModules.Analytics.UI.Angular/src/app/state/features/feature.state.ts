import { State } from '@ngxs/store';
import { Injectable } from '@angular/core';

export interface FeaturesStateModel {
}

@State<FeaturesStateModel>({
  name: 'features',
  defaults: {
  },
  children: [
  ]
})
@Injectable()
export class FeaturesState {
}

