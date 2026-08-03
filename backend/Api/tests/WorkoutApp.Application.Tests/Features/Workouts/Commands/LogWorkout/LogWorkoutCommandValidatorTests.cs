using WorkoutApp.Application.DTOs.Workout.LogWorkout;
using WorkoutApp.Application.Features.Workouts.Commands.LogWorkout;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.Tests.Features.Workouts.Commands.LogWorkout;

public class LogWorkoutCommandValidatorTests
{
    private readonly LogWorkoutCommandValidator _validator = new();

    private static LogWorkoutRequest ValidRequest() => new(
        ExerciseType.Cardio, 30, 200, 5, 5, "Felt good.", DateTime.UtcNow.AddDays(-1));

    private static LogWorkoutCommand CommandWith(Func<LogWorkoutRequest, LogWorkoutRequest> mutate) =>
        new(mutate(ValidRequest()));

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var result = _validator.Validate(CommandWith(r => r));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithUndefinedExerciseType_HasErrorForExerciseType()
    {
        var result = _validator.Validate(CommandWith(r => r with { ExerciseType = (ExerciseType)999 }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.ExerciseType");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveDuration_HasErrorForDuration(int duration)
    {
        var result = _validator.Validate(CommandWith(r => r with { DurationMinutes = duration }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.DurationMinutes");
    }

    [Fact]
    public void Validate_WithNegativeCalories_HasErrorForCalories()
    {
        var result = _validator.Validate(CommandWith(r => r with { CaloriesBurned = -1 }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.CaloriesBurned");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Validate_WithDifficultyOutOfRange_HasErrorForDifficulty(int difficulty)
    {
        var result = _validator.Validate(CommandWith(r => r with { Difficulty = difficulty }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Difficulty");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Validate_WithFatigueOutOfRange_HasErrorForFatigue(int fatigue)
    {
        var result = _validator.Validate(CommandWith(r => r with { Fatigue = fatigue }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Fatigue");
    }

    [Fact]
    public void Validate_WithNotesExceedingMaxLength_HasErrorForNotes()
    {
        var result = _validator.Validate(CommandWith(r => r with { Notes = new string('a', 501) }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Notes");
    }

    [Fact]
    public void Validate_WithFutureDate_HasErrorForPerformedAt()
    {
        var result = _validator.Validate(CommandWith(r => r with { PerformedAt = DateTime.UtcNow.AddDays(1) }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.PerformedAt");
    }
}