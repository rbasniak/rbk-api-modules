import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AnalyticsEntry } from '@models/analytics/analytics-entry';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { FilteringOptionsSelectors } from '@state/database/analytics/filtering-options/filtering-options.selectors';
import { SearchFeatureActions } from '@state/features/analytics/search/search.actions';
import { SearchFeatureSelectors } from '@state/features/analytics/search/search.selectors';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzForm, SmzMultiSelectControl, SmzTextControl } from 'ngx-smz-dialogs';
import { SmzTableBuilder, SmzTableState } from 'ngx-smz-ui';
import { Observable } from 'rxjs';
import { ChartDefinition } from '@models/chart-definition';
import * as moment_ from 'moment';

const moment = moment_;

@UntilDestroy()
@Component({
  selector: 'app-search-page',
  templateUrl: 'search-page.component.html',
  styleUrls: ['search-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SearchPageComponent implements OnInit {
  @Select(SearchFeatureSelectors.results) public results$: Observable<AnalyticsEntry[]>;
  @Select(SearchFeatureSelectors.charts) public charts$: Observable<ChartDefinition[]>;

  public formConfig: SmzForm<unknown> = null;
  public tableConfig: SmzTableState = null;

  constructor(private store: Store) {
  }

  public ngOnInit(): void {
    this.setupForm();
    this.setupTable();
  }

  private setupTable(): void {
    this.tableConfig = new SmzTableBuilder()
      .usePagination()
      .setPaginationDefaultRows(10)
      .setPaginationPageOptions([25, 50, 100])
      .columns()
        .custom('response', '', '4em')
          .ignoreOnGlobalFilter()
          .disableFilter()
          .disableSort()
          .columns
        .custom('timestamp', 'TIMESTAMP', '9em')
          .disableFilter()
          .disableSort()
          .columns
        .custom('action', 'ENDPOINT')
          .disableFilter()
          .disableSort()
          .columns
        .custom('application', 'APPLICATION')
          .disableFilter()
          .disableSort()
          .columns
        .custom('user', 'USER')
          .disableFilter()
          .disableSort()
          .columns
        .custom('performance', 'PERFORMANCE', '20em')
          .disableFilter()
          .disableSort()
          .columns
        .table
      .build();
  }

  private setupForm(): void {
    const dateFrom: SmzCalendarControl = {
      propertyName: 'dateFrom', type: SmzControlType.CALENDAR, name: 'From', defaultValue: moment(new Date()).subtract(30,'days').toDate(),
      touchUI: false, focusTrap: true, keepInvalid: true, showButtonBar: false, showOnFocus: false, showIcon: true,
      template: { extraLarge: { row: 'col-2' } }
    };

    const dateTo: SmzCalendarControl = {
      propertyName: 'dateTo', type: SmzControlType.CALENDAR, name: 'To', defaultValue: new Date(),
      touchUI: false, focusTrap: true, keepInvalid: true, showButtonBar: false, showOnFocus: false, showIcon: true,
      template: { extraLarge: { row: 'col-2' } }
    };

    const version: SmzMultiSelectControl<string> = {
      propertyName: 'versions', type: SmzControlType.MULTI_SELECT, name: 'Application Versions', validatorsPreset: { isRequired: false }, defaultLabel: 'All versions',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.versions),
      template: { extraLarge: { row: 'col-2' } }
    };

    const area: SmzMultiSelectControl<string> = {
      propertyName: 'areas', type: SmzControlType.MULTI_SELECT, name: 'Application Areas', validatorsPreset: { isRequired: false }, defaultLabel: 'All areas',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.areas),
      template: { extraLarge: { row: 'col-2' } }
    };

    const domain: SmzMultiSelectControl<string> = {
      propertyName: 'domains', type: SmzControlType.MULTI_SELECT, name: 'Domains', validatorsPreset: { isRequired: false }, defaultLabel: 'All domains',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.domains),
      template: { extraLarge: { row: 'col-2' } }
    };

    const endpoint: SmzMultiSelectControl<string> = {
      propertyName: 'actions', type: SmzControlType.MULTI_SELECT, name: 'Endpoints', validatorsPreset: { isRequired: false }, defaultLabel: 'All endpoints',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.endpoints),
      template: { extraLarge: { row: 'col-3' } }
    };

    const response: SmzMultiSelectControl<string> = {
      propertyName: 'responses', type: SmzControlType.MULTI_SELECT, name: 'Responses', validatorsPreset: { isRequired: false }, defaultLabel: 'All responses',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.responses),
      template: { extraLarge: { row: 'col-2' } }
    };

    const user: SmzMultiSelectControl<string> = {
      propertyName: 'users', type: SmzControlType.MULTI_SELECT, name: 'Users', validatorsPreset: { isRequired: false }, defaultLabel: 'All users',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.users),
      template: { extraLarge: { row: 'col-2' } }
    };

    const agent: SmzMultiSelectControl<string> = {
      propertyName: 'agents', type: SmzControlType.MULTI_SELECT, name: 'User Agents', validatorsPreset: { isRequired: false }, defaultLabel: 'All user agents',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.userAgents),
      template: { extraLarge: { row: 'col-4' } }
    };

    const elementId: SmzTextControl = {
      propertyName: 'entityId', type: SmzControlType.TEXT, name: 'Entity Id', validatorsPreset: { isRequired: false },
      defaultValue: null,
      template: { extraLarge: { row: 'col-3' } }
    };

    this.formConfig = {
      behaviors: { flattenResponse: true, avoidFocusOnLoad: true },
      groups: [
        {
          name: null, showName: false,
          children: [ dateFrom, dateTo, version, area, domain, user, endpoint, response, agent, elementId ],
          template: { extraLarge: { row: 'col-12' } }
        },
      ],
    };
  }

  public search(event: FormGroupComponent): void {
    const response = event.getData().data as any;
    for (const key in response) {
      if (key.endsWith('Ids')) {
        const newKey = key.substring(0, key.length - 3);
        response[newKey] = response[key];
        delete response[key];
      }
    }

    this.store.dispatch(new SearchFeatureActions.Search(response));
  }
}