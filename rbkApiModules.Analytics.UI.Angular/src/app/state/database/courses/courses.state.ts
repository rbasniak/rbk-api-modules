// import { State, Action, StateContext, Store } from '@ngxs/store';
// import { Observable } from 'rxjs';
// import { tap } from 'rxjs/operators';
// import { Injectable } from '@angular/core';
// import { CoursesDetails } from '@models/courses-details';
// import { getInitialDatabaseStoreState, replaceArrayItem } from 'ngx-rbk-utils';
// import { CoursesService } from '@services/api/courses.service';
// import { CoursesActions } from './courses.actions';

// export const COURSES_STATE_NAME = 'courses';

// export interface CoursesStateModel {
//   lastUpdated: Date | null;
//   items: CoursesDetails[];
// }

// @State<CoursesStateModel>({
//   name: COURSES_STATE_NAME,
//   defaults: getInitialDatabaseStoreState<CoursesDetails>()
// })
// @Injectable()
// export class CoursesState {

//   constructor(private coursesService: CoursesService, private store: Store) { }

//   @Action(CoursesActions.LoadAll)
//   public loadAll$(ctx: StateContext<CoursesStateModel>): Observable<CoursesDetails[]> {
//     return this.coursesService.all().pipe(
//       tap((result: CoursesDetails[]) => {
//         ctx.patchState({
//           lastUpdated: new Date(),
//           items: result,
//         });
//       })
//     );
//   }

//   @Action(CoursesActions.Create)
//   public create$(ctx: StateContext<CoursesStateModel>, action: CoursesActions.Create): Observable<CoursesDetails> {
//     return this.coursesService.create(action.data).pipe(
//       tap((result: CoursesDetails) => {
//         ctx.patchState({
//           lastUpdated: new Date(),
//           items: [result, ...ctx.getState().items],
//         });
//       })
//     );
//   }

//   @Action(CoursesActions.Update)
//   public update$(ctx: StateContext<CoursesStateModel>, action: CoursesActions.Update): Observable<CoursesDetails> {
//     return this.coursesService.update(action.data).pipe(
//       tap((result: CoursesDetails) => {
//         ctx.patchState({
//           lastUpdated: new Date(),
//           items: replaceArrayItem(ctx.getState().items, result),
//         });
//       })
//     );
//   }

//   @Action(CoursesActions.Delete)
//   public delete$(ctx: StateContext<CoursesStateModel>, action: CoursesActions.Delete): Observable<void> {
//     return this.coursesService.delete(action.id).pipe(
//       tap(() => {
//         ctx.patchState({
//           lastUpdated: new Date(),
//           items: ctx.getState().items.filter(x => x.id !== action.id),
//         });
//       })
//     );
//   }
// }
