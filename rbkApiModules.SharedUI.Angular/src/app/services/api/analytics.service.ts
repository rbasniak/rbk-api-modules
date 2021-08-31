import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { BaseApiService } from 'ngx-smz-ui';
import { environment } from '@environments/environment';
import { FilteringOptions } from '@models/analytics/filtering-options';
import { FilterData } from '@models/analytics/filter-data';
import { fixDates } from 'ngx-smz-ui';
import { SearchResults } from '@models/analytics/search-results';
import { ChartDefinition } from '@models/chart-definition';
import { SessionsData } from '@models/analytics/sessions-data';
import { PerformanceData } from '@models/analytics/performance-data';
import { getUrl } from '@services/utils';

@Injectable({ providedIn: 'root' })
export class AnalyticsService extends BaseApiService {
  private endpoint = `${getUrl()}/api/analytics`;

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

  public getPerformanceData(endpoint: string, dateFrom: Date, dateTo: Date, groupingType: number): Observable<PerformanceData> {
    return this.http.post<PerformanceData>(`${this.endpoint}/performance`, { endpoint, dateFrom, dateTo, groupingType }, this.generateDefaultHeaders({}));
  }

  public getSessionsData(dateFrom: Date, dateTo: Date, groupingType: number): Observable<SessionsData> {
    return this.http.post<SessionsData>(`${this.endpoint}/sessions`, { dateFrom, dateTo, groupingType }, this.generateDefaultHeaders({}));
  }

  public deleteBasedOnPathText(searchText: string): Observable<void> {
    return this.http.post<void>(`${this.endpoint}/delete-matching-path`, { searchText }, this.generateDefaultHeaders({}));
  }
}

