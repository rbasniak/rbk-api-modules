import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { BaseApiService } from 'ngx-rbk-utils';
import { environment } from '@environments/environment';
import { FilteringOptions } from '@models/filtering-options';
import { FilterData } from '@models/filter-data';
import { AnalyticsEntry } from '@models/analytics-entry';
import { ChartResponse } from '@models/chart-response';
import { fixDates } from 'ngx-rbk-utils';

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

  public search(data: FilterData): Observable<AnalyticsEntry[]> {
    return this.http.post<AnalyticsEntry[]>(`${this.endpoint}/search`, data, this.generateDefaultHeaders({})).pipe(
      fixDates()
    );
  }

  public getDashboardData(dateFrom: Date, dateTo: Date, groupingType: number): Observable<ChartResponse[]> {
    return this.http.post<ChartResponse[]>(`${this.endpoint}/dashboard`, { dateFrom, dateTo, groupingType }, this.generateDefaultHeaders({}));
  }
}

