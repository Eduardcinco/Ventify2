import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AiMessage { role: 'system' | 'user' | 'assistant'; content: string; }

@Injectable({ providedIn: 'root' })
export class AiService {
  private base = `${environment.apiUrl}/ai`;
  constructor(private http: HttpClient) {}

  chat(payload: { model?: string; messages: AiMessage[] }): Observable<{ message: string }> {
    const body = {
      model: payload.model,
      messages: payload.messages.map(m => ({ role: m.role, content: m.content }))
    };
    return this.http.post<{ message: string }>(`${this.base}/chat`, body, { withCredentials: true });
  }
}
