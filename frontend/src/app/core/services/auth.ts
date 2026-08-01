import { Service, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenStorage } from './token-storage';
import { RegisterRequest } from '../../shared/dto/requests/auth/register-request';
import { RegisterResponse } from '../../shared/dto/responses/auth/register-response';
import { LoginRequest } from '../../shared/dto/requests/auth/login-request';
import { LoginResponse } from '../../shared/dto/responses/auth/login-response';
import { CurrentUser } from '../../shared/models/current-user';

interface DecodedToken {
  sub: string;
  email: string;
  exp: number;
}

@Service()
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenStorage = inject(TokenStorage);

  private readonly currentUserSignal = signal<CurrentUser | null>(this.decodeStoredToken());
  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUserSignal() !== null);

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http
      .post<RegisterResponse>(`${environment.apiUrl}/auth/register`, request)
      .pipe(tap((response) => this.handleAuthResponse(response.token)));
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(tap((response) => this.handleAuthResponse(response.token)));
  }

  logout(): void {
    this.tokenStorage.clearToken();
    this.currentUserSignal.set(null);
  }

  private handleAuthResponse(token: string): void {
    this.tokenStorage.setToken(token);
    this.currentUserSignal.set(this.decodeToken(token));
  }

  private decodeStoredToken(): CurrentUser | null {
    const token = this.tokenStorage.getToken();
    return token ? this.decodeToken(token) : null;
  }

  private decodeToken(token: string): CurrentUser | null {
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      return { userId: decoded.sub, email: decoded.email };
    } catch {
      return null;
    }
  }
}