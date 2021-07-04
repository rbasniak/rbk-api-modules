import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';
import { map } from 'rxjs/operators';

@UntilDestroy()
@Component({
  selector: 'app-home',
  templateUrl: 'home.component.html',
  styleUrls: ['home.component.scss']
})
export class HomeComponent implements OnInit {
  public text = 'Loading file contents...';

  constructor(private http: HttpClient) {
    this.http.get('https://raw.githubusercontent.com/rbasniak/rbk-api-modules/master/README.md', {responseType: 'text'}).pipe(
      map(x => this.text = x)
    ).subscribe();
  }
  public ngOnInit(): void {
  }
}