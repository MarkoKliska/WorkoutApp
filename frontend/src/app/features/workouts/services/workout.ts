import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LogWorkoutRequest } from '../../../shared/dto/requests/workouts/log-workout-request';
import { LogWorkoutResponse } from '../../../shared/dto/responses/workouts/log-workout-response';
import { WorkoutResponse } from '../../../shared/dto/responses/workouts/workout-response';

@Service()
export class WorkoutService {
  private readonly http = inject(HttpClient);

  logWorkout(request: LogWorkoutRequest): Observable<LogWorkoutResponse> {
    return this.http.post<LogWorkoutResponse>(`${environment.apiUrl}/workouts`, request);
  }

  getWorkouts(): Observable<WorkoutResponse[]> {
    return this.http.get<WorkoutResponse[]>(`${environment.apiUrl}/workouts`);
  }
}