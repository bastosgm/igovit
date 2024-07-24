import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {}

  login(credentials: { username: string; password: string }) {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      withCredentials: true,
    };

    return this.http
      .post<Response>(`${this.baseUrl}api/Login`, credentials, httpOptions)
      .pipe(
        tap((response) => {
          // Adicione lógica de pós-login aqui, se necessário
          console.log('Login bem-sucedido', response);
        })
      );
  }

  logout() {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      withCredentials: true, // Garante que cookies sejam enviados e recebidos
    };

    return this.http
      .post<Response>(`${this.baseUrl}api/Logout`, {}, httpOptions)
      .pipe(
        tap((response) => {
          // Adicione lógica de pós-logout aqui, se necessário
          console.log('Logout bem-sucedido', response);
        })
      );
  }

  isAuthenticated(): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      withCredentials: true, // Garante que cookies sejam enviados e recebidos
    };

    return this.http.get<boolean>(
      `${this.baseUrl}api/IsAuthenticated`,
      httpOptions
    );
  }

  // getToken(): string | null {

  // }
}
