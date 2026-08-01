import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { TokenStorage } from '../services/token-storage';
import { RouteNames } from '../../shared/consts/routes';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenStorage = inject(TokenStorage);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && req.headers.has('Authorization')) {
        tokenStorage.clearToken();
        router.navigate(['/', RouteNames.Login]);
      }

      return throwError(() => error);
    }),
  );
};