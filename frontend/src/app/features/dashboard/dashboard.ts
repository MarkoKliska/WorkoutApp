import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { WorkoutService } from '../workouts/services/workout';
import { MonthlyProgressResponse } from '../../shared/dto/responses/workouts/monthly-progress-response';
import { RouteNames } from '../../shared/consts/routes';
import { DatePipe, DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, DatePipe, DecimalPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly workoutService = inject(WorkoutService);

  protected readonly routeNames = RouteNames;

  private readonly today = new Date();
  protected readonly selectedYear = signal(this.today.getFullYear());
  protected readonly selectedMonth = signal(this.today.getMonth() + 1);
  protected readonly progress = signal<MonthlyProgressResponse | null>(null);

  protected readonly monthInputValue = computed(
    () => `${this.selectedYear()}-${this.selectedMonth().toString().padStart(2, '0')}`,
  );

  constructor() {
    this.loadProgress();
  }

  onMonthChange(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    if (!value) {
      return;
    }

    const [year, month] = value.split('-').map(Number);
    this.selectedYear.set(year);
    this.selectedMonth.set(month);
    this.loadProgress();
  }

  private loadProgress(): void {
    this.workoutService
      .getMonthlyProgress(this.selectedYear(), this.selectedMonth())
      .subscribe((progress) => this.progress.set(progress));
  }
}