import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenStorage } from '../services/token-storage';
import { RouteNames } from '../../shared/consts/routes';

export const noAuthGuard: CanActivateFn = () => {
  const tokenStorage = inject(TokenStorage);
  const router = inject(Router);

  if (!tokenStorage.hasValidToken()) {
    return true;
  }

  return router.createUrlTree(['/', RouteNames.Dashboard]);
};