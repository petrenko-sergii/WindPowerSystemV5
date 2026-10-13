import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { AiChatAnswer } from './ai-chat';

@Injectable({
  providedIn: 'root',
})
export class AiChatService {
  constructor(private http: HttpClient) {
  }

  askAboutTurbineTypes(question: string): Observable<AiChatAnswer> {
    var url = environment.baseUrl + "api/ai-chat/turbine-types";

    return this.http.post<AiChatAnswer>(url, { question });
  }
}
