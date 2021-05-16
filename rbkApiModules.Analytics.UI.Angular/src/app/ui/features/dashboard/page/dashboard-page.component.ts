import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AnalyticsEntry } from '@models/analytics-entry';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { DashboardFeatureActions } from '@state/features/dashboard/dashboard.actions';
import { DashboardFeatureSelectors } from '@state/features/dashboard/dashboard.selectors';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzDropDownControl, SmzForm } from 'ngx-smz-dialogs';
import { Observable } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'app-dashboard-page',
  templateUrl: 'dashboard-page.component.html',
  styleUrls: ['dashboard-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class DashboardPageComponent implements OnInit {
  @Select(DashboardFeatureSelectors.data) public data$: Observable<AnalyticsEntry[]>;

  public formConfig: SmzForm<unknown> = null;

  constructor(private store: Store) {
  }

  public ngOnInit(): void {
    // moment(new Date()).subtract(1,'months').endOf('month').toDate()
    const dateFrom: SmzCalendarControl = {
      propertyName: 'dateFrom', type: SmzControlType.CALENDAR, name: 'From', defaultValue: new Date(),
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
    console.log(response);
    this.store.dispatch(new DashboardFeatureActions.Filter(response.dateFrom, response.dateTo, response.grouping));
  }
}