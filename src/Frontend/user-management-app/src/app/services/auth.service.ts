import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser, DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LoginDto, LoginResponse, User } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:55930/api';
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private isLoggedIn = false; // Track login state
  private token: string | null = null;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object,
    @Inject(DOCUMENT) private document: Document
  ) {
    // Restaurar estado de autenticação na inicialização
    if (isPlatformBrowser(this.platformId)) {
      const storedToken = sessionStorage.getItem('auth-token');
      if (storedToken && !this.isTokenExpired(storedToken)) {
        this.token = storedToken;
        this.isLoggedIn = true;
        const user = this.getUserFromToken(storedToken);
        if (user) {
          this.currentUserSubject.next(user);
        }
      }
    }
  }

  login(loginData: LoginDto): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/auth/login`, loginData, {
        withCredentials: true,
      })
      .pipe(
        tap((response) => {
          this.isLoggedIn = true;
          this.token = response.token;
          this.currentUserSubject.next(response.user);

          // Armazenar token no sessionStorage
          if (isPlatformBrowser(this.platformId)) {
            sessionStorage.setItem('auth-token', response.token);
          }
        })
      );
  }

  logout(): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/auth/logout`, {}, { withCredentials: true })
      .pipe(
        tap(() => {
          this.isLoggedIn = false;
          this.token = null;
          this.currentUserSubject.next(null);

          // Remover token do sessionStorage
          if (isPlatformBrowser(this.platformId)) {
            sessionStorage.removeItem('auth-token');
          }
        })
      );
  }

  isAuthenticated(): boolean {
    const hasUser = !!this.currentUserSubject.value;
    return this.isLoggedIn && hasUser;
  }

  getToken(): string | null {
    return this.token;
  }

  private getCookie(name: string): string | null {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }

    const value = `; ${this.document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
      return parts.pop()?.split(';').shift() || null;
    }
    return null;
  }

  private getUserFromToken(token: string): User | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return {
        id: payload.nameid,
        name: payload.unique_name,
        email: payload.email,
        createdAt: new Date(),
      };
    } catch {
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const now = Math.floor(Date.now() / 1000);
      return payload.exp < now;
    } catch {
      return true;
    }
  }
}
