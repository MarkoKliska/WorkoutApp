import { Routes } from '@angular/router';
import { noAuthGuard } from '../../core/guards/no-auth.guard';
import { RouteNames } from '../../shared/consts/routes';

export const authRoutes: Routes = [
  {
    path: RouteNames.Login,
    loadComponent: () => import('./login/login').then((m) => m.Login),
    canActivate: [noAuthGuard],
  },
  {
    path: RouteNames.Register,
    loadComponent: () => import('./register/register').then((m) => m.Register),
    canActivate: [noAuthGuard],
  },
];