namespace WorkoutApp.Application.DTOs.Workout.GetMonthlyProgress;

public sealed record MonthlyProgressResponse(int Year, int Month, IReadOnlyList<WeeklyProgressResponse> Weeks);