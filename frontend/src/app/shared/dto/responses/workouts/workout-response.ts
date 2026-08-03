import { ExerciseType } from '../../../models/exercise-type';

export interface WorkoutResponse {
  id: string;
  exerciseType: ExerciseType;
  durationMinutes: number;
  caloriesBurned: number;
  difficulty: number;
  fatigue: number;
  notes?: string;
  performedAt: string;
}