import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Turbine } from './turbine';
import { TurbineType } from '../turbine-types/turbine-type';
import { environment } from '../../environments/environment';

// This will tell Angular to provide this injectable in the application root,
// thus making it a singleton service.
@Injectable({
  providedIn: 'root',
})
export class TurbineService {
  constructor(protected http: HttpClient) {
  }

  // REST Approach
  getDataRestApproach(): Observable<Turbine[]> {
    const url = this.getUrl('api/turbines');
    return this.http.get<Turbine[]>(url);
  }

  getData(): Observable<Turbine[]> {
    const url = this.getUrl('api/graphql');
    const query = `
    query {
      turbines {
        id
        serialNumber
        status
        turbineTypeId
        manufacturer
        model
      }
    }
  `;
    return this.http.post<any>(url, { query }).pipe(
      // The GraphQL response will be { data: { turbines: Turbine[] } }
      // so we map to just the array of turbines
      map(response => response.data.turbines as Turbine[])
    );
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
