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
  getData(): Observable<TurbineType[]> {
    const url = this.getUrl('api/turbine-types');
    return this.http.get<TurbineType[]>(url);
  }

  // GraphQL Approach
  getDataWithGraphQLApproach(): Observable<TurbineType[]> {
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

  // REST Approach
  putWithRestApproach(item: TurbineType): Observable<TurbineType> {
    const url = this.getUrl('api/turbine-types/' + item.id);
    return this.http.put<TurbineType>(url, item);
  }

  put(input: TurbineType): Observable<TurbineType> {
    return this.apollo
      .mutate({
        mutation: gql`
          mutation UpdateTurbineType($turbineType: TurbineTypeInput!) {
            updateTurbineType(turbineType: $turbineType) {
              id
              manufacturer
              model
              capacity
            }
          }
        `,
        variables: {
          turbineType: input
        }
      })
      .pipe(map((result: any) => result.data.updateTurbineType));
  }

  post(item: TurbineType): Observable<TurbineType> {
    const url = this.getUrl('api/turbine-types');
    return this.http.post<TurbineType>(url, item);
  }

  protected getUrl(url: string) {
    return environment.baseUrl + url;
  }
}
