import { Routes } from '@angular/router';
import { RouteNames } from '../../shared/consts/routes';

export const workoutsRoutes: Routes = [
  {
    path: RouteNames.Workouts,
    loadComponent: () => import('./workout-list/workout-list').then((m) => m.WorkoutList),
  },
  {
    path: RouteNames.LogWorkout,
    loadComponent: () => import('./log-workout/log-workout').then((m) => m.LogWorkout),
  },
];