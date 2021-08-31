import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { DiagnosticsEntry } from '@models/diagnostics/diagnostics-entry';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { FilteringOptionsSelectors } from '@state/database/diagnostics/filtering-options/filtering-options.selectors';
import { SearchFeatureActions } from '@state/features/diagnostics/search/search.actions';
import { SearchFeatureSelectors } from '@state/features/diagnostics/search/search.selectors';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzForm, SmzMultiSelectControl, SmzTextControl } from 'ngx-smz-ui';
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
  @Select(SearchFeatureSelectors.results) public results$: Observable<DiagnosticsEntry[]>;
  @Select(SearchFeatureSelectors.charts) public charts$: Observable<ChartDefinition[]>;

  public formConfig: SmzForm<unknown> = null;
  public tableConfig: SmzTableState = null;
  public emptyObject = '{...}';

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
      .useStrippedStyle()
      .columns()
        .custom('timestamp', 'RESULTS', '9em')
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

    const layer: SmzMultiSelectControl<string> = {
      propertyName: 'layers', type: SmzControlType.MULTI_SELECT, name: 'Layers', validatorsPreset: { isRequired: false }, defaultLabel: 'All application layers',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.layers),
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

    const user: SmzMultiSelectControl<string> = {
      propertyName: 'users', type: SmzControlType.MULTI_SELECT, name: 'Users', validatorsPreset: { isRequired: false }, defaultLabel: 'All users',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.users),
      template: { extraLarge: { row: 'col-2' } }
    };

    const browser: SmzMultiSelectControl<string> = {
      propertyName: 'browsers', type: SmzControlType.MULTI_SELECT, name: 'Browsers', validatorsPreset: { isRequired: false }, defaultLabel: 'All browsers',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.browsers),
      template: { extraLarge: { row: 'col-3' } }
    };

    const agent: SmzMultiSelectControl<string> = {
      propertyName: 'agents', type: SmzControlType.MULTI_SELECT, name: 'User Agents', validatorsPreset: { isRequired: false }, defaultLabel: 'All user agents',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.userAgents),
      template: { extraLarge: { row: 'col-3' } }
    };

    const device: SmzMultiSelectControl<string> = {
      propertyName: 'devices', type: SmzControlType.MULTI_SELECT, name: 'Devices', validatorsPreset: { isRequired: false }, defaultLabel: 'All devices',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.devices),
      template: { extraLarge: { row: 'col-2' } }
    };

    const os: SmzMultiSelectControl<string> = {
      propertyName: 'operatingSystems', type: SmzControlType.MULTI_SELECT, name: 'Operating Systems', validatorsPreset: { isRequired: false }, defaultLabel: 'All operating systems',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.operatingSystems),
      template: { extraLarge: { row: 'col-2' } }
    };

    const source: SmzMultiSelectControl<string> = {
      propertyName: 'sources', type: SmzControlType.MULTI_SELECT, name: 'Sources', validatorsPreset: { isRequired: false }, defaultLabel: 'All sources',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.sources),
      template: { extraLarge: { row: 'col-3' } }
    };

    const message: SmzMultiSelectControl<string> = {
      propertyName: 'messages', type: SmzControlType.MULTI_SELECT, name: 'Messages', validatorsPreset: { isRequired: false }, defaultLabel: 'All messages',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.messages),
      template: { extraLarge: { row: 'col-6' } }
    };

    const text: SmzTextControl = {
      propertyName: 'text', type: SmzControlType.TEXT, name: 'Contains Text', validatorsPreset: { isRequired: false },
      defaultValue: null,
      template: { extraLarge: { row: 'col-3' } }
    };

    this.formConfig = {
      behaviors: { flattenResponse: true, avoidFocusOnLoad: true },
      groups: [
        {
          name: null, showName: false,
          children: [ dateFrom, dateTo, version, layer, area, domain, user, browser, agent, device, os, source, message, text ],
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

  public toggleExpansion(item: any, id: string): void {
    if (item[`${id}Expanded`] == null) {
      item[`${id}Expanded`] = true;
    }
    else {
      item[`${id}Expanded`] = !item[`${id}Expanded`];
    }
  }
}