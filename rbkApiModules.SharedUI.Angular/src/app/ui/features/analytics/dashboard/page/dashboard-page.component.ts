import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AnalyticsEntry } from '@models/analytics/analytics-entry';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { DashboardFeatureActions } from '@state/features/analytics/dashboard/dashboard.actions';
import { DashboardFeatureSelectors } from '@state/features/analytics/dashboard/dashboard.selectors';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzDropDownControl, SmzForm } from 'ngx-smz-ui';
import { Observable } from 'rxjs';
import * as moment_ from 'moment';

const moment = moment_;

@UntilDestroy()
@Component({
  selector: 'app-dashboard-page',
  templateUrl: 'dashboard-page.component.html',
  styleUrls: ['dashboard-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DashboardPageComponent implements OnInit {
  @Select(DashboardFeatureSelectors.data) public data$: Observable<any>;

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
    this.store.dispatch(new DashboardFeatureActions.Filter(response.dateFrom, response.dateTo, response.groupingId));
  }
}