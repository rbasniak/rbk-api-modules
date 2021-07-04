import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { BaseApiService } from 'ngx-rbk-utils';
import { environment } from '@environments/environment';
import { MenuData } from '@models/menu-info';

@Injectable({ providedIn: 'root' })
export class MenuService extends BaseApiService {
  private endpoint = `${window.location.origin}/api/shared-ui`;

  constructor(private http: HttpClient) {
    super();

    if (!environment.production) {
      this.endpoint = `${environment.serverUrl}/api/shared-ui`;
    }
  }

  public load(): Observable<MenuData> {
    return this.http.get<MenuData>(`${this.endpoint}/menu`, this.generateDefaultHeaders({
      authentication: false,
      needToRefreshToken: false,
    }));
  }
}