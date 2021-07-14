import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Store } from '@ngxs/store';
import { SmzForm } from 'ngx-smz-dialogs';
import * as moment_ from 'moment';

const moment = moment_;

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
  }
}