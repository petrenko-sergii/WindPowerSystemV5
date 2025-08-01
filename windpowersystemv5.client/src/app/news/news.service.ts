import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { News } from './news';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class NewsService {
  constructor(
    protected http: HttpClient
  ) { }

  getAll(): Observable<News[]> {
    const url = this.getUrl('api/News');
    return this.http.get<News[]>(url);
  }

  protected getUrl(url: string) {
    return environment.baseUrl + url;
  }

  get(id: string): Observable<News> {
    const url = this.getUrl(`api/News/${id}`);
    return this.http.get<News>(url);
  }
}
