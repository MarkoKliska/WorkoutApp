import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { authRoutes } from './features/auth/auth.routes';
import { workoutsRoutes } from './features/workouts/workouts.routes';
import { RouteNames } from './shared/consts/routes';
import { usersRoutes } from './features/users/users.routes';

export const routes: Routes = [
  ...authRoutes,
  {
    path: '',
    loadComponent: () =>
      import('./shared/ui/authenticated-layout/authenticated-layout').then((m) => m.AuthenticatedLayout),
        canActivate: [authGuard],
        children: [
          {
            path: RouteNames.Dashboard,
            loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
          },
          ...workoutsRoutes,
          ...usersRoutes,
          { path: '', pathMatch: 'full', redirectTo: RouteNames.Dashboard },
        ],
  },
  { path: '**', redirectTo: RouteNames.Dashboard },
];