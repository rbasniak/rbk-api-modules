import { State, Action, StateContext, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { MenuService } from '@services/api/menu.service';
import { MenuData } from '@models/menu-info';
import { MenuActions } from './menu.actions';

export const MENU_STATE_NAME = 'menu';

export interface MenuStateModel {
  lastUpdated: Date | null;
  data: MenuData;
}

@State<MenuStateModel>({
  name: MENU_STATE_NAME,
  defaults: { lastUpdated: null, data: null }
})
@Injectable()
export class MenuState {

  constructor(private menuService: MenuService, private store: Store) { }

  @Action(MenuActions.LoadAll)
  public loadAll$(ctx: StateContext<MenuStateModel>): Observable<MenuData> {
    return this.menuService.load().pipe(
      tap((result: MenuData) => {
        ctx.patchState({
          lastUpdated: new Date(),
          data: result,
        });
      })
    );
  }
}
