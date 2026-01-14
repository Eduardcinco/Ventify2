import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

interface AuthResponse {
  accessToken?: string;
  refreshToken?: string;
  usuario?: any;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = `${environment.apiUrl}/auth`;
  private logged$ = new BehaviorSubject<boolean>(false);
  private session: any = null;

  constructor(private http: HttpClient) {}

  login(payload: { email: string; password: string }): Observable<AuthResponse> {
    const body = { Correo: payload.email, Password: payload.password };
    return this.http.post<AuthResponse>(`${this.base}/login`, body, { withCredentials: true }).pipe(
      tap(res => {
        this.session = res.usuario || null;
        this.logged$.next(true);
      })
    );
  }

  register(payload: { name: string; email: string; password: string; businessName?: string }): Observable<AuthResponse> {
    const body: any = { Nombre: payload.name, Correo: payload.email, Password: payload.password };
    return this.http.post<AuthResponse>(`${this.base}/register`, body, { withCredentials: true }).pipe(
      tap(res => {
        this.session = res.usuario || null;
        this.logged$.next(true);
      })
    );
  }

  logout(): Observable<any> {
    this.session = null;
    this.logged$.next(false);
    return this.http.post(`${this.base}/logout`, {}, { withCredentials: true });
  }

  isAuthenticated(): boolean {
    return this.logged$.value;
  }

  getSessionSync() {
    return this.session;
  }

  sessionChanges() {
    return this.logged$.asObservable();
  }
}
