import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseApiService } from 'ngx-rbk-utils';
import { environment } from '@environments/environment';
import { fixDates } from 'ngx-rbk-utils';
import { ChartDefinition } from '@models/chart-definition';
import { FilteringOptions } from '@models/diagnostics/filtering-options';
import { SearchResults } from '@models/diagnostics/search-results';
import { FilterData } from '@models/diagnostics/filter-data';

@Injectable({ providedIn: 'root' })
export class DiagnosticsService extends BaseApiService {
  private endpoint = `${window.location.origin}/api/diagnostics`;

  constructor(private http: HttpClient) {
    super();

    if (!environment.production) {
      this.endpoint = `${environment.serverUrl}/api/diagnostics`;
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