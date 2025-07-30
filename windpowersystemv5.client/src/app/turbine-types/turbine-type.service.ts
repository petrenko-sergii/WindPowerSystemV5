import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Apollo, gql } from 'apollo-angular';
import { lastValueFrom } from 'rxjs';

import { TurbineType } from './turbine-type';
import { environment } from '../../environments/environment';

// This will tell Angular to provide this injectable in the application root,
// thus making it a singleton service.
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

  downloadInfoFile(fileName: string): void {
    const params = new HttpParams().set('fileName', fileName);
    this.http.get(`api/turbine-types/download-info-file`, {
      params,
      responseType: 'blob',
      observe: 'response'
    }).subscribe({
      next: (response) => {
        const filename = this.getFileNameFromContentDisposition(response);
        const blob = new Blob([response.body as BlobPart], { type: response.body?.type });

        // Create download link
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = filename ?? 'downloaded-file';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      },
      error: (err) => {
        console.error('File download failed', err);
      }
    });
  }

  getInfoFile(fileName: string): Promise<File | null> {
    return lastValueFrom(
      this.http.get(`api/turbine-types/download-info-file`, {
        params: { fileName },
        responseType: 'blob'
      })
    ).then((blob) => {
      if (fileName.toLowerCase().endsWith('.jpg') && blob) {
        return new File([blob], fileName, { type: 'image/jpeg' });
      }
      else if (fileName.toLowerCase().endsWith('.pdf') && blob) {
        return new File([blob], fileName, { type: 'application/pdf' });
      }
      return null;
    }).catch(() => null);
  }

  private getFileNameFromContentDisposition(response: any): string | null {
    const contentDisposition = response.headers.get('Content-Disposition');
    if (contentDisposition) {
      const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
      if (matches && matches[1]) {
        return matches[1].replace(/['"]/g, '');
      }
    }
    return null;
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
              fileName
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

  createWithInfoFile(item: TurbineType, infoFile: File ): Observable<TurbineType> {
    const url = this.getUrl('api/turbine-types');

    const formData = new FormData();
    formData.append('manufacturer', item.manufacturer);
    formData.append('model', item.model);
    formData.append('capacity', item.capacity.toString());
    formData.append('infoFile', infoFile, infoFile.name);

    return this.http.post<TurbineType>(url, formData);
  }

  protected getUrl(url: string) {
    return environment.baseUrl + url;
  }
}
