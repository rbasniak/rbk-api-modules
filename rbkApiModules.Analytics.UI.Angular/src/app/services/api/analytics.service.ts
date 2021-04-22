import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { BaseApiService } from 'ngx-rbk-utils';
import { environment } from '@environments/environment';

@Injectable({ providedIn: 'root' })
export class AnalyticsService extends BaseApiService {
  private endpoint = `${window.location.origin}/api/analytics`;

  constructor(private http: HttpClient) {
    super();

    if (!environment.production) {
      this.endpoint = `${environment.serverUrl}/api/analytics`;
    }

    console.log(window.location);
    console.log(this.endpoint);
  }

  public test(): Observable<string[]> {
    return this.http.get<string[]>(`${this.endpoint}/test`, this.generateDefaultHeaders({}));
  }
}

