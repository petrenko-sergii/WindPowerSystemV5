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

  getAllNews(): Observable<News[]> {
    const url = this.getUrl('api/News');
    return this.http.get<News[]>(url);
  }

  protected getUrl(url: string) {
    return environment.baseUrl + url;
  }
}
