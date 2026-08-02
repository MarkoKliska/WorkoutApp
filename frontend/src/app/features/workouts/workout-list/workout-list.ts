import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { WorkoutService } from '../services/workout';
import { WorkoutResponse } from '../../../shared/dto/responses/workouts/workout-response';
import { RouteNames } from '../../../shared/consts/routes';

@Component({
  selector: 'app-workout-list',
  imports: [DatePipe, RouterLink],
  templateUrl: './workout-list.html',
  styleUrl: './workout-list.scss',
})
export class WorkoutList {
  private readonly workoutService = inject(WorkoutService);

  protected readonly routeNames = RouteNames;
  protected readonly workouts = signal<WorkoutResponse[]>([]);
  protected readonly isLoaded = signal(false);

  constructor() {
    this.workoutService.getWorkouts().subscribe((workouts) => {
      this.workouts.set(workouts);
      this.isLoaded.set(true);
    });
  }
}