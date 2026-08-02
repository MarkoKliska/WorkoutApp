using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.DTOs.Workout.LogWorkout;

public sealed record LogWorkoutRequest(
    ExerciseType ExerciseType,
    int DurationMinutes,
    int CaloriesBurned,
    int Difficulty,
    int Fatigue,
    string? Notes,
    DateTime PerformedAt);