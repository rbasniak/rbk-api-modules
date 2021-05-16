import { Component, OnInit } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { AnalyticsService } from '@services/api/analytics.service';

@UntilDestroy()
@Component({
  selector: 'app-home',
  templateUrl: 'home.component.html',
  styleUrls: ['home.component.scss']
})
export class HomeComponent implements OnInit {
  public items = [];
  constructor(private service: AnalyticsService) {

  }
  public ngOnInit(): void {
  }

  public test(): void {
    // this.service.test().subscribe(x => {
    //   console.log(x);
    //   this.items = x;
    // });
  }
}