import { Service } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { StorageKeys } from '../../shared/consts/storage-keys';

@Service()
export class TokenStorage {
  getToken(): string | null {
    return localStorage.getItem(StorageKeys.AuthToken);
  }

  setToken(token: string): void {
    localStorage.setItem(StorageKeys.AuthToken, token);
  }

  clearToken(): void {
    localStorage.removeItem(StorageKeys.AuthToken);
  }

  hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    try {
      const { exp } = jwtDecode<{ exp?: number }>(token);
      return exp === undefined || exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }
}