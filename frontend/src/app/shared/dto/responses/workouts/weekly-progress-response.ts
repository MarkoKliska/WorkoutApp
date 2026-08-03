export interface WeeklyProgressResponse {
  weekStart: string;
  weekEnd: string;
  workoutCount: number;
  totalDurationMinutes: number;
  averageDifficulty: number;
  averageFatigue: number;
}