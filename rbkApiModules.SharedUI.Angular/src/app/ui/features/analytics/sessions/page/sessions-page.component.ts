import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzDropDownControl, SmzForm } from 'ngx-smz-ui';
import * as moment_ from 'moment';
import { SessionsFeatureSelectors } from '@state/features/analytics/sessions/sessions.selectors';
import { Observable } from 'rxjs';
import { SessionsFeatureActions } from '@state/features/analytics/sessions/sessions.actions';

const moment = moment_;

@UntilDestroy()
@Component({
  selector: 'app-sessions-page',
  templateUrl: 'sessions-page.component.html',
  styleUrls: ['sessions-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SessionsPageComponent implements OnInit {
  @Select(SessionsFeatureSelectors.data) public data$: Observable<any>;

  public formConfig: SmzForm<unknown> = null;

  constructor(private store: Store) {
  }

  public ngOnInit(): void {
    const dateFrom: SmzCalendarControl = {
      propertyName: 'dateFrom', type: SmzControlType.CALENDAR, name: 'From', defaultValue: moment(new Date()).subtract(30,'days').toDate(),
      touchUI: false, focusTrap: true, keepInvalid: true, showButtonBar: false, showOnFocus: false, showIcon: true,
      template: { extraLarge: { row: 'col-4' } }
    };

    const dateTo: SmzCalendarControl = {
      propertyName: 'dateTo', type: SmzControlType.CALENDAR, name: 'To', defaultValue: new Date(),
      touchUI: false, focusTrap: true, keepInvalid: true, showButtonBar: false, showOnFocus: false, showIcon: true,
      template: { extraLarge: { row: 'col-4' } }
    };

    const grouping: SmzDropDownControl<number> = {
      propertyName: 'grouping', type: SmzControlType.DROPDOWN, name: 'Grouping', validatorsPreset: { isRequired: true },
      defaultValue: 1, showFilter: false, options: [
        { id: 1, name: 'Daily'},
        { id: 3, name: 'Weekly'},
        { id: 4, name: 'Monthly'},
        { id: 5, name: 'Yearly'},
      ],
      template: { extraLarge: { row: 'col-4' } }
    };

    this.formConfig = {
      behaviors: { flattenResponse: true, avoidFocusOnLoad: true },
      groups: [
        {
          name: null, showName: true,
          children: [ dateFrom, dateTo, grouping ],
          template: { extraLarge: { row: 'col-12' } }
        },
      ],
    };
  }

  public filter(event: FormGroupComponent): void {
    const response = event.getData().data as any;
    this.store.dispatch(new SessionsFeatureActions.LoadAll(response.dateFrom, response.dateTo, response.groupingId));
  }
}