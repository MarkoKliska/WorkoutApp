import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { WorkoutService } from '../services/workout';
import { ToastService } from '../../../core/services/toast';
import { RouteNames } from '../../../shared/consts/routes';
import { ExerciseType } from '../../../shared/models/exercise-type';

@Component({
  selector: 'app-log-workout',
  imports: [ReactiveFormsModule],
  templateUrl: './log-workout.html',
  styleUrl: './log-workout.scss',
})
export class LogWorkout {
  private readonly fb = inject(FormBuilder);
  private readonly workoutService = inject(WorkoutService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly isSubmitting = signal(false);
  protected readonly exerciseTypes = Object.values(ExerciseType);
  protected readonly maxPerformedAt = this.toLocalDateTimeInputValue(new Date());

  protected readonly form = this.fb.nonNullable.group({
    exerciseType: [ExerciseType.Cardio, Validators.required],
    durationMinutes: [30, [Validators.required, Validators.min(1)]],
    caloriesBurned: [0, [Validators.required, Validators.min(0)]],
    difficulty: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
    fatigue: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
    notes: ['', Validators.maxLength(500)],
    performedAt: [this.toLocalDateTimeInputValue(new Date()), Validators.required],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const { notes, performedAt, ...rest } = this.form.getRawValue();

    this.workoutService
      .logWorkout({ ...rest, notes: notes || undefined, performedAt: new Date(performedAt).toISOString() })
      .subscribe({
        next: () => {
          this.toastService.success('Workout logged!');
          this.router.navigate(['/', RouteNames.Dashboard]);
        },
        error: (error) => {
          this.isSubmitting.set(false);
          this.toastService.error(error.error?.detail ?? 'Please check your details and try again.');
        },
      });
  }

  private toLocalDateTimeInputValue(date: Date): string {
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }
}