import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Apollo, gql } from 'apollo-angular'; 

import { Turbine } from './turbine';
import { TurbineType } from '../turbine-types/turbine-type';
import { environment } from '../../environments/environment';

// This will tell Angular to provide this injectable in the application root,
// thus making it a singleton service.
@Injectable({
  providedIn: 'root',
})
export class TurbineService {
  constructor(
    protected http: HttpClient,
    private apollo: Apollo
  ) { }

  // REST Approach
  getDataRestApproach(): Observable<Turbine[]> {
    const url = this.getUrl('api/turbines');
    return this.http.get<Turbine[]>(url);
  }

  // GraphQL Approach, with Apollo
  getData(): Observable<Turbine[]> {
    return this.apollo
      .query({
        query: gql`
        query GetTurbines {
          turbineQueries {
            turbines {
              id
              serialNumber
              status
              turbineTypeId
              manufacturer
              model
            }
          }
        }
      `
      })
      .pipe(map((result: any) => result.data.turbineQueries.turbines));
  }

  // GraphQL Approach, without Apollo
  getData2(): Observable<Turbine[]> {
    const url = this.getUrl('api/graphql');
    const query = `
    query {
      turbineQueries {
        turbines {
          id
          serialNumber
          status
          turbineTypeId
          manufacturer
          model
        }
      }
    }
  `;
    return this.http.post<any>(url, { query }).pipe(
      // The GraphQL response will be { data: { turbineQueries : { turbines: Turbine[] } } }
      // so we map to just the array of turbines
      map(response => response.data.turbineQueries.turbines as Turbine[])
    );
  }

  // REST Approach
  getRestApproach(id: number): Observable<Turbine> {
    const url = this.getUrl('api/turbines/' + id);
    return this.http.get<Turbine>(url);
  }

  get(id: number): Observable<Turbine> {
    return this.apollo
      .query({
        query: gql`
          query GetTurbine($id: Int!) {
            turbineQueries {
              turbine(id: $id) {
                id
                serialNumber
                status
                turbineTypeId
              }
            }
          }
        `,
        variables: {
          id
        }
      })
      .pipe(map((result: any) => result.data.turbineQueries.turbine));
  }

  // REST Approach
  putRestApproach(item: Turbine): Observable<Turbine> {
    const url = this.getUrl('api/turbines/' + item.id);
    return this.http.put<Turbine>(url, item);
  }

  put(item: Turbine): Observable<Turbine> {
    return this.apollo
      .mutate({
        mutation: gql`
          mutation UpdateTurbine($turbine: TurbineUpdateRequestInput!) {
           turbineMutations {
             updateTurbine(turbine: $turbine) {
               id
               serialNumber
               status
               turbineTypeId
             }
           }
          }
        `,
        variables: {
          turbine: item
        }
      })
      .pipe(map((result: any) => result.data.turbineMutations.updateTurbine));
  }

  // REST Approach
  postRestApproach(item: Turbine): Observable<Turbine> {
    const url = this.getUrl('api/turbines');
    return this.http.post<Turbine>(url, item);
  }

  post(item: Turbine): Observable<Turbine> {
    return this.apollo
      .mutate({
        mutation: gql`
          mutation AddTurbine($turbine: TurbineCreationRequestInput!) {
            turbineMutations {
              addTurbine(turbine: $turbine) {
                id
                serialNumber
                status
                turbineTypeId
              }
            }
          }
        `,
        variables: {
          turbine: item
        }
      })
      .pipe(map((result: any) => result.data.turbineMutations.addTurbine));
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
