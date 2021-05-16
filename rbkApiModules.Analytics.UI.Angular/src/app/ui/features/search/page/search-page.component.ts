import { Component, OnInit } from '@angular/core';
import { AnalyticsEntry } from '@models/analytics-entry';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Select, Store } from '@ngxs/store';
import { FilteringOptionsSelectors } from '@state/database/filtering-options/filtering-options.selectors';
import { SearchFeatureActions } from '@state/features/search/search.actions';
import { SearchFeatureSelectors } from '@state/features/search/search.selectors';
import { FormGroupComponent, SmzCalendarControl, SmzControlType, SmzForm, SmzMultiSelectControl, SmzTextControl } from 'ngx-smz-dialogs';
import { SmzContentType, SmzFilterType, SmzTableConfig } from 'ngx-smz-ui';
import { Observable } from 'rxjs';

@UntilDestroy()
@Component({
  selector: 'app-search-page',
  templateUrl: 'search-page.component.html',
  styleUrls: ['search-page.component.scss']
})
export class SearchPageComponent implements OnInit {
  @Select(SearchFeatureSelectors.results) public results$: Observable<AnalyticsEntry[]>;

  public formConfig: SmzForm<unknown> = null;
  public tableConfig: SmzTableConfig = null;

  constructor(private store: Store) {
  }

  public ngOnInit(): void {
    this.setupForm();
    this.setupTable2();
  }

  private setupTable2(): void {
    this.tableConfig = {
      showPaginator: true,
      rowsPerPageOptions: [25, 50, 100],
      showActions: true,
      showGlobalFilter: true,
      showCaption: true,
      columns: [
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'response',
          header: '',
          isGlobalFilterable: false,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
          width: '5em',
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'timestamp',
          header: 'TIMESTAMP',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'action',
          header: 'ENDPOINT',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'application',
          header: 'APPLICATION',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'user',
          header: 'USER',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'performance',
          header: 'PERFORMANCE',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },

      ]
    };
  }

  private setupTable1(): void {
    this.tableConfig = {
      showPaginator: true,
      rowsPerPageOptions: [25, 50, 100],
      showActions: true,
      showGlobalFilter: true,
      showCaption: true,
      columns: [
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'response',
          header: '',
          isGlobalFilterable: false,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
          width: '3em',
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'timestamp',
          header: 'TIMESTAMP',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'action',
          header: 'ENDPOINT',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'application',
          header: 'APPLICATION',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'user',
          header: 'USER',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },
        {
          contentType: SmzContentType.TEXT,
          contentData: { useTemplate: true },
          field: 'performance',
          header: 'PERFORMANCE',
          isGlobalFilterable: true,
          isOrderable: false,
          showFilter: false,
          isVisible: true,
        },

      ]
    };
  }

  private setupForm(): void {
    // moment(new Date()).subtract(1,'months').endOf('month').toDate()
    const dateFrom: SmzCalendarControl = {
      propertyName: 'dateFrom', type: SmzControlType.CALENDAR, name: 'From', defaultValue: new Date(),
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
      propertyName: 'endpoints', type: SmzControlType.MULTI_SELECT, name: 'Endpoints', validatorsPreset: { isRequired: false }, defaultLabel: 'All endpoints',
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