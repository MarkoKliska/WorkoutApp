namespace WorkoutApp.Application.DTOs.Workout.GetMonthlyProgress;

public sealed record WeeklyProgressResponse(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    int WorkoutCount,
    int TotalDurationMinutes,
    double AverageDifficulty,
    double AverageFatigue);