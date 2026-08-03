using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Enums;
using WorkoutApp.Domain.ValueObjects;

namespace WorkoutApp.Domain.Entities;

public sealed class Workout : AggregateRoot, ISoftDeletable
{
    private const int MaxNotesLength = 500;

    public Guid UserId { get; private set; }
    public ExerciseType ExerciseType { get; private set; }
    public int DurationMinutes { get; private set; }
    public int CaloriesBurned { get; private set; }
    public Rating Difficulty { get; private set; } = null!;
    public Rating Fatigue { get; private set; } = null!;
    public string? Notes { get; private set; }
    public DateTime PerformedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    private Workout(
        Guid id, Guid userId, ExerciseType exerciseType, int durationMinutes, int caloriesBurned,
        Rating difficulty, Rating fatigue, string? notes, DateTime performedAt, DateTimeOffset createdAt)
        : base(id)
    {
        UserId = userId;
        ExerciseType = exerciseType;
        DurationMinutes = durationMinutes;
        CaloriesBurned = caloriesBurned;
        Difficulty = difficulty;
        Fatigue = fatigue;
        Notes = notes;
        PerformedAt = performedAt;
        CreatedAt = createdAt;
    }

    private Workout() { }

    public static Result<Workout> Log(
        Guid userId, ExerciseType exerciseType, int durationMinutes, int caloriesBurned,
        int difficulty, int fatigue, string? notes, DateTime performedAt)
    {
        if (durationMinutes <= 0)
            return Result.Failure<Workout>(Error.Validation("Workout.InvalidDuration", "Duration must be greater than zero."));

        if (caloriesBurned < 0)
            return Result.Failure<Workout>(Error.Validation("Workout.InvalidCalories", "Calories burned cannot be negative."));

        if (notes is { Length: > MaxNotesLength })
            return Result.Failure<Workout>(Error.Validation("Workout.NotesTooLong", $"Notes cannot exceed {MaxNotesLength} characters."));

        if (performedAt > DateTime.UtcNow)
            return Result.Failure<Workout>(Error.Validation("Workout.FutureDate", "Workout date cannot be in the future."));

        var difficultyResult = Rating.Create(difficulty);
        if (difficultyResult.IsFailure)
            return Result.Failure<Workout>(difficultyResult.Error);

        var fatigueResult = Rating.Create(fatigue);
        if (fatigueResult.IsFailure)
            return Result.Failure<Workout>(fatigueResult.Error);

        return Result.Success(new Workout(
            Guid.NewGuid(), userId, exerciseType, durationMinutes, caloriesBurned,
            difficultyResult.Value, fatigueResult.Value, notes, performedAt, DateTimeOffset.UtcNow));
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}