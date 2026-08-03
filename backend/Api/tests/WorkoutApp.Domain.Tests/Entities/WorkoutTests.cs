using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Domain.Tests.Entities;

public class WorkoutTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private static readonly DateTime ValidPerformedAt = DateTime.UtcNow.AddDays(-1);

    private static Result<Workout> LogValidWorkout(
        ExerciseType exerciseType = ExerciseType.Cardio,
        int durationMinutes = 30,
        int caloriesBurned = 200,
        int difficulty = 5,
        int fatigue = 5,
        string? notes = "Felt good.",
        DateTime? performedAt = null) =>
        Workout.Log(ValidUserId, exerciseType, durationMinutes, caloriesBurned, difficulty, fatigue, notes, performedAt ?? ValidPerformedAt);

    [Fact]
    public void Log_WithValidData_ReturnsSuccessWithExpectedProperties()
    {
        var result = LogValidWorkout();

        Assert.True(result.IsSuccess);
        var workout = result.Value;
        Assert.Equal(ValidUserId, workout.UserId);
        Assert.Equal(ExerciseType.Cardio, workout.ExerciseType);
        Assert.Equal(30, workout.DurationMinutes);
        Assert.Equal(200, workout.CaloriesBurned);
        Assert.Equal(5, workout.Difficulty.Value);
        Assert.Equal(5, workout.Fatigue.Value);
        Assert.Equal("Felt good.", workout.Notes);
        Assert.Equal(ValidPerformedAt, workout.PerformedAt);
        Assert.False(workout.IsDeleted);
        Assert.Null(workout.DeletedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Log_WithNonPositiveDuration_ReturnsFailure(int durationMinutes)
    {
        var result = LogValidWorkout(durationMinutes: durationMinutes);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.InvalidDuration", result.Error.Code);
    }

    [Fact]
    public void Log_WithNegativeCalories_ReturnsFailure()
    {
        var result = LogValidWorkout(caloriesBurned: -1);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.InvalidCalories", result.Error.Code);
    }

    [Fact]
    public void Log_WithNotesAtMaxLength_ReturnsSuccess()
    {
        var notes = new string('a', 500);

        var result = LogValidWorkout(notes: notes);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Log_WithNotesExceedingMaxLength_ReturnsFailure()
    {
        var notes = new string('a', 501);

        var result = LogValidWorkout(notes: notes);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.NotesTooLong", result.Error.Code);
    }

    [Fact]
    public void Log_WithNullNotes_ReturnsSuccess()
    {
        var result = LogValidWorkout(notes: null);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Notes);
    }

    [Fact]
    public void Log_WithFutureDate_ReturnsFailure()
    {
        var result = LogValidWorkout(performedAt: DateTime.UtcNow.AddDays(1));

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.FutureDate", result.Error.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Log_WithDifficultyOutOfRange_ReturnsFailure(int difficulty)
    {
        var result = LogValidWorkout(difficulty: difficulty);

        Assert.True(result.IsFailure);
        Assert.Equal("Rating.OutOfRange", result.Error.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Log_WithFatigueOutOfRange_ReturnsFailure(int fatigue)
    {
        var result = LogValidWorkout(fatigue: fatigue);

        Assert.True(result.IsFailure);
        Assert.Equal("Rating.OutOfRange", result.Error.Code);
    }

    [Fact]
    public void Delete_SetsIsDeletedAndDeletedAt()
    {
        var workout = LogValidWorkout().Value;

        workout.Delete();

        Assert.True(workout.IsDeleted);
        Assert.NotNull(workout.DeletedAt);
    }
}