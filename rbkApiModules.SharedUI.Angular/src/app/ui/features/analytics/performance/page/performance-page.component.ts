import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { Store } from '@ngxs/store';
import { SmzForm } from 'ngx-smz-ui';
import * as moment_ from 'moment';

const moment = moment_;

@UntilDestroy()
@Component({
  selector: 'app-performance-page',
  templateUrl: 'performance-page.component.html',
  styleUrls: ['performance-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PerformancePageComponent implements OnInit {
  public formConfig: SmzForm<unknown> = null;

  constructor(private store: Store) {
  }

  public ngOnInit(): void {
  }
}