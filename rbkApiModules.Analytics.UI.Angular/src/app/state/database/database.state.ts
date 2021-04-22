import { State } from '@ngxs/store';
import { Injectable } from '@angular/core';

export interface DatabaseStateModel {
}

@State<DatabaseStateModel>({
  name: 'database',
  defaults: {
  },
  children: [
  ]
})
@Injectable()
export class DatabaseState {
}

