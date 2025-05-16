import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { TurbineType } from './turbine-type';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TurbineTypeService {
  constructor(protected http: HttpClient) {
  }

  getData(): Observable<TurbineType[]> {
    const url = this.getUrl('api/turbine-types');
    return this.http.get<TurbineType[]>(url);
  }

  get(id: number): Observable<TurbineType> {
    const url = this.getUrl('api/turbine-types/' + id);
    return this.http.get<TurbineType>(url);
  }

  put(item: TurbineType): Observable<TurbineType> {
    const url = this.getUrl('api/turbine-types/' + item.id);
    return this.http.put<TurbineType>(url, item);
  }

  post(item: TurbineType): Observable<TurbineType> {
    const url = this.getUrl('api/turbine-types');
    return this.http.post<TurbineType>(url, item);
  }

  protected getUrl(url: string) {
    return environment.baseUrl + url;
  }
}
