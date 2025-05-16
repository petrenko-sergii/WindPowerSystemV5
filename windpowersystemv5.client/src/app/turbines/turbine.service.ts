import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Turbine } from './turbine';
import { TurbineType } from '../turbine-types/turbine-type';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TurbineService {
  constructor(protected http: HttpClient) {
  }

  getData(): Observable<Turbine[]> {
    const url = this.getUrl('api/turbines');
    return this.http.get<Turbine[]>(url);
  }

  get(id: number): Observable<Turbine> {
    const url = this.getUrl('api/turbines/' + id);
    return this.http.get<Turbine>(url);
  }

  put(item: Turbine): Observable<Turbine> {
    const url = this.getUrl('api/turbines/' + item.id);
    return this.http.put<Turbine>(url, item);
  }

  post(item: Turbine): Observable<Turbine> {
    const url = this.getUrl('api/turbines');
    return this.http.post<Turbine>(url, item);
  }

  getTurbineTypes(): Observable<TurbineType[]> {
    const url = this.getUrl('api/turbine-types');
    return this.http.get<TurbineType[]>(url);
  }

  loadStatuses(): Observable<string[]> {
    const url = this.getUrl('api/turbines/statuses');
    return this.http.get<string[]>(url);
  }

  protected getUrl(url: string) {
    return environment.baseUrl + url;
  }
}
