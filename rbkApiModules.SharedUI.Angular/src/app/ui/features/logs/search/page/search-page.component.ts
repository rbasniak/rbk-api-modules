import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { LogLevel, LogsEntry } from '@models/logs/logs-entry';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { FilteringOptionsSelectors } from '@state/database/logs/filtering-options/filtering-options.selectors';
import { SearchFeatureActions } from '@state/features/logs/search/search.actions';
import { SearchFeatureSelectors } from '@state/features/logs/search/search.selectors';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzForm, SmzMultiSelectControl } from 'ngx-smz-ui';
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
  @Select(SearchFeatureSelectors.results) public results$: Observable<LogsEntry[]>;
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

    const message: SmzMultiSelectControl<string> = {
      propertyName: 'messages', type: SmzControlType.MULTI_SELECT, name: 'Messages', validatorsPreset: { isRequired: false }, defaultLabel: 'All messages',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.messages),
      template: { extraLarge: { row: 'col-6' } }
    };

    const level: SmzMultiSelectControl<LogLevel> = {
      propertyName: 'levels', type: SmzControlType.MULTI_SELECT, name: 'Levels', validatorsPreset: { isRequired: false }, defaultLabel: 'All levels',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.levels),
      template: { extraLarge: { row: 'col-6' } }
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

    const version: SmzMultiSelectControl<string> = {
      propertyName: 'versions', type: SmzControlType.MULTI_SELECT, name: 'Application Versions', validatorsPreset: { isRequired: false }, defaultLabel: 'All versions',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.versions),
      template: { extraLarge: { row: 'col-2' } }
    };

    const source: SmzMultiSelectControl<string> = {
      propertyName: 'sources', type: SmzControlType.MULTI_SELECT, name: 'Sources', validatorsPreset: { isRequired: false }, defaultLabel: 'All sources',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.sources),
      template: { extraLarge: { row: 'col-3' } }
    };

    const enviroment: SmzMultiSelectControl<string> = {
      propertyName: 'enviroments', type: SmzControlType.MULTI_SELECT, name: 'Enviroments', validatorsPreset: { isRequired: false }, defaultLabel: 'All operating systems',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.enviroments),
      template: { extraLarge: { row: 'col-2' } }
    };

    const enviromentVersion: SmzMultiSelectControl<string> = {
      propertyName: 'enviromentsVersions', type: SmzControlType.MULTI_SELECT, name: 'Enviroments Versions', validatorsPreset: { isRequired: false }, defaultLabel: 'All operating systems',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.enviromentsVersions),
      template: { extraLarge: { row: 'col-2' } }
    };

    const user: SmzMultiSelectControl<string> = {
      propertyName: 'users', type: SmzControlType.MULTI_SELECT, name: 'Users', validatorsPreset: { isRequired: false }, defaultLabel: 'All users',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.users),
      template: { extraLarge: { row: 'col-2' } }
    };

    const domain: SmzMultiSelectControl<string> = {
      propertyName: 'domains', type: SmzControlType.MULTI_SELECT, name: 'Domains', validatorsPreset: { isRequired: false }, defaultLabel: 'All domains',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.domains),
      template: { extraLarge: { row: 'col-2' } }
    };

    const machine: SmzMultiSelectControl<string> = {
      propertyName: 'machines', type: SmzControlType.MULTI_SELECT, name: 'Machines', validatorsPreset: { isRequired: false }, defaultLabel: 'All machines',
      defaultValue: null, showFilter: true, options: this.store.selectSnapshot(FilteringOptionsSelectors.machines),
      template: { extraLarge: { row: 'col-3' } }
    };

    this.formConfig = {
      behaviors: { flattenResponse: true, avoidFocusOnLoad: true },
      groups: [
        {
          name: null, showName: false,
          children: [ dateFrom, dateTo, message, level, layer, area, version, source, enviroment, enviromentVersion, user, domain, machine ],
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