import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Store } from '@ngxs/store';
import { AdminFeatureActions } from '@state/features/analytics/admin/admin.actions';
import { Confirmable, FormGroupComponent, SmzControlType, SmzForm, SmzTextControl } from 'ngx-smz-ui';

@UntilDestroy()
@Component({
  selector: 'app-admin-page',
  templateUrl: 'admin-page.component.html',
  styleUrls: ['admin-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AdminPageComponent implements OnInit {
  public formConfig: SmzForm<unknown> = null;

  constructor(private store: Store) {
  }

  public ngOnInit(): void {
    const searchText: SmzTextControl = {
      propertyName: 'searchText', type: SmzControlType.TEXT, name: 'Matching text', defaultValue: '',
      template: { extraLarge: { row: 'col-12' } }
    };

    this.formConfig = {
      behaviors: { flattenResponse: true, avoidFocusOnLoad: true },
      groups: [
        {
          name: null, showName: true,
          children: [ searchText ],
          template: { extraLarge: { row: 'col-12' } }
        },
      ],
    };
  }

  @Confirmable('Are you sure you want to continue? Deleted data cannot be recovered', 'Confirmation', true)
  public execute(event: FormGroupComponent): void {
    const response = event.getData().data as any;
    this.store.dispatch(new AdminFeatureActions.DeleteBasedOnPathText(response.searchText));
  }
}