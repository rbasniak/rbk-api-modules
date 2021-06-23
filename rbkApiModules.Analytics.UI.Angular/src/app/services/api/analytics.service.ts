import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { BaseApiService } from 'ngx-rbk-utils';
import { environment } from '@environments/environment';
import { FilteringOptions } from '@models/filtering-options';
import { FilterData } from '@models/filter-data';
import { fixDates } from 'ngx-rbk-utils';
import { SearchResults } from '@models/search-results';
import { ChartDefinition } from '@models/chart-definition';

@Injectable({ providedIn: 'root' })
export class AnalyticsService extends BaseApiService {
  private endpoint = `${window.location.origin}/api/analytics`;

  constructor(private http: HttpClient) {
    super();

    if (!environment.production) {
      this.endpoint = `${environment.serverUrl}/api/analytics`;
    }
  }

  public getFilteringOptions(): Observable<FilteringOptions> {
    return this.http.get<FilteringOptions>(`${this.endpoint}/filter-options`, this.generateDefaultHeaders({}));
  }

  public search(data: FilterData): Observable<SearchResults> {
    return this.http.post<SearchResults>(`${this.endpoint}/search`, data, this.generateDefaultHeaders({})).pipe(
      fixDates()
    );
  }

  public getDashboardData(dateFrom: Date, dateTo: Date, groupingType: number): Observable<ChartDefinition[]> {
    return this.http.post<ChartDefinition[]>(`${this.endpoint}/dashboard`, { dateFrom, dateTo, groupingType }, this.generateDefaultHeaders({}));
  }
}

