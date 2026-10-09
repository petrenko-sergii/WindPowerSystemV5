import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { WeatherClothing } from './weather';

@Injectable({
  providedIn: 'root',
})
export class WeatherService {
  constructor(private http: HttpClient) {
  }

  getClothingAdvice(city: string, lat: number, lon: number): Observable<WeatherClothing> {
    var url = environment.baseUrl + "api/Weather/clothing";
    var params = new HttpParams()
      .set("city", city)
      .set("lat", lat.toString())
      .set("lon", lon.toString());

    return this.http.get<WeatherClothing>(url, { params });
  }
}
