import { WeeklyProgressResponse } from './weekly-progress-response';

export interface MonthlyProgressResponse {
  year: number;
  month: number;
  weeks: WeeklyProgressResponse[];
}