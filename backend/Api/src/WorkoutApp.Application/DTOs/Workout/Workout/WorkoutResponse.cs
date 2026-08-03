using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.DTOs.Workout;

public sealed record WorkoutResponse(
    Guid Id,
    ExerciseType ExerciseType,
    int DurationMinutes,
    int CaloriesBurned,
    int Difficulty,
    int Fatigue,
    string? Notes,
    DateTime PerformedAt);