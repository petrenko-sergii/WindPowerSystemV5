import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Apollo, gql } from 'apollo-angular'; 

import { TurbineType } from './turbine-type';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TurbineTypeService {
  constructor(
    protected http: HttpClient,
    private apollo: Apollo // Add Apollo to the constructor
  ) { }

  // REST Approach
  getDataWithRestApproach(): Observable<TurbineType[]> {
    const url = this.getUrl('api/turbine-types');
    return this.http.get<TurbineType[]>(url);
  }

  getData(): Observable<TurbineType[]> {
    return this.apollo
      .query({
        query: gql`
        query GetTurbineTypes {
          turbineTypes {
            id
            manufacturer
            model
            capacity
            turbineQty
          }
        }
      `
      })
      .pipe(map((result: any) => result.data.turbineTypes));
  }

  // REST Approach
  getWithRestApproach(id: number): Observable<TurbineType> {
    const url = this.getUrl('api/turbine-types/' + id);
    return this.http.get<TurbineType>(url);
  }

  get(id: number): Observable<TurbineType> {
    return this.apollo
      .query({
        query: gql`
          query GetTurbineType($id: Int!) {
            turbineType(id: $id) {
              id
              manufacturer
              model
              capacity
            }
          }
        `,
        variables: {
          id
        }
      })
      .pipe(map((result: any) => result.data.turbineType));
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
