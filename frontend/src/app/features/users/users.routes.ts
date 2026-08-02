import { Routes } from '@angular/router';
import { RouteNames } from '../../shared/consts/routes';

export const usersRoutes: Routes = [
  {
    path: RouteNames.Profile,
    loadComponent: () => import('./profile-page/profile-page').then((m) => m.ProfilePage),
  },
];