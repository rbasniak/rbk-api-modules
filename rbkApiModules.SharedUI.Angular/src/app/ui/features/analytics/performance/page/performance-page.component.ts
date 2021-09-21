import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzDropDownControl, SmzForm } from 'ngx-smz-ui';
import * as moment_ from 'moment';
import { Observable } from 'rxjs';
import { FilteringOptionsSelectors } from '@state/database/analytics/filtering-options/filtering-options.selectors';
import { PerformanceFeatureActions } from '@state/features/analytics/performance/performance.actions';
import { PerformanceFeatureSelectors } from '@state/features/analytics/performance/performance.selectors';

const moment = moment_;

@UntilDestroy()
@Component({
  selector: 'app-performance-page',
  templateUrl: 'performance-page.component.html',
  styleUrls: ['performance-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PerformancePageComponent implements OnInit {
  @Select(PerformanceFeatureSelectors.data) public data$: Observable<any>;

  public formConfig: SmzForm<unknown> = null;

  constructor(private store: Store) {
  }

  public ngOnInit(): void {
    const dateFrom: SmzCalendarControl = {
      propertyName: 'dateFrom', type: SmzControlType.CALENDAR, name: 'From', defaultValue: moment(new Date()).subtract(30,'days').toDate(),
      touchUI: false, focusTrap: true, keepInvalid: true, showButtonBar: false, showOnFocus: false, showIcon: true,
      template: { extraLarge: { row: 'col-3' } }
    };

    const dateTo: SmzCalendarControl = {
      propertyName: 'dateTo', type: SmzControlType.CALENDAR, name: 'To', defaultValue: new Date(),
      touchUI: false, focusTrap: true, keepInvalid: true, showButtonBar: false, showOnFocus: false, showIcon: true,
      template: { extraLarge: { row: 'col-3' } }
    };

    const actions = this.store.selectSnapshot(FilteringOptionsSelectors.endpoints);
    actions.shift();

    const endpoint: SmzDropDownControl<string> = {
      propertyName: 'actions', type: SmzControlType.DROPDOWN, name: 'Endpoints', validatorsPreset: { isRequired: true },
      defaultValue: actions[0], showFilter: true, options: actions,
      template: { extraLarge: { row: 'col-3' } }
    };

    const grouping: SmzDropDownControl<number> = {
      propertyName: 'grouping', type: SmzControlType.DROPDOWN, name: 'Grouping', validatorsPreset: { isRequired: true },
      defaultValue: 1, showFilter: false, options: [
        { id: 1, name: 'Daily'},
        { id: 3, name: 'Weekly'},
        { id: 4, name: 'Monthly'},
        { id: 5, name: 'Yearly'},
      ],
      template: { extraLarge: { row: 'col-3' } }
    };

    this.formConfig = {
      behaviors: { flattenResponse: true, avoidFocusOnLoad: true },
      groups: [
        {
          name: null, showName: true,
          children: [ dateFrom, dateTo, endpoint, grouping ],
          template: { extraLarge: { row: 'col-12' } }
        },
      ],
    };
  }

  public filter(event: FormGroupComponent): void {
    const response = event.getData().data as any;
    this.store.dispatch(new PerformanceFeatureActions.LoadAll(response.actionsId, response.dateFrom, response.dateTo, response.groupingId));
  }
}