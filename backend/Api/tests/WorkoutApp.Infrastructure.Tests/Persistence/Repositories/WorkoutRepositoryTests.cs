using Microsoft.EntityFrameworkCore;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Enums;
using WorkoutApp.Infrastructure.Persistence.Repositories;

namespace WorkoutApp.Infrastructure.Tests.Persistence.Repositories;

public class WorkoutRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenWorkoutIsSoftDeleted_ReturnsNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var workout = Workout.Log(Guid.NewGuid(), ExerciseType.Cardio, 30, 200, 5, 5, null, DateTime.UtcNow.AddDays(-1)).Value;
        workout.Delete();
        context.Workouts.Add(workout);
        await context.SaveChangesAsync();
        var repository = new WorkoutRepository(context);

        var result = await repository.GetByIdAsync(workout.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyThatUsersWorkouts_OrderedByPerformedAtDescending()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var older = Workout.Log(userId, ExerciseType.Cardio, 30, 200, 5, 5, null, DateTime.UtcNow.AddDays(-5)).Value;
        var newer = Workout.Log(userId, ExerciseType.Strength, 40, 300, 6, 6, null, DateTime.UtcNow.AddDays(-1)).Value;
        var othersWorkout = Workout.Log(Guid.NewGuid(), ExerciseType.Cardio, 20, 150, 3, 2, null, DateTime.UtcNow.AddDays(-2)).Value;
        context.Workouts.AddRange(older, newer, othersWorkout);
        await context.SaveChangesAsync();
        var repository = new WorkoutRepository(context);

        var result = await repository.GetByUserAsync(userId);

        Assert.Equal([newer.Id, older.Id], result.Select(w => w.Id));
    }

    [Fact]
    public async Task GetByUserAndDateRangeAsync_FiltersToInclusiveRange()
    {
        await using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var inRange = Workout.Log(userId, ExerciseType.Cardio, 30, 200, 5, 5, null, new DateTime(2020, 1, 15, 0, 0, 0, DateTimeKind.Utc)).Value;
        var outOfRange = Workout.Log(userId, ExerciseType.Cardio, 30, 200, 5, 5, null, new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc)).Value;
        context.Workouts.AddRange(inRange, outOfRange);
        await context.SaveChangesAsync();
        var repository = new WorkoutRepository(context);

        var result = await repository.GetByUserAndDateRangeAsync(
            userId, new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2020, 1, 31, 23, 59, 59, DateTimeKind.Utc));

        var found = Assert.Single(result);
        Assert.Equal(inRange.Id, found.Id);
    }

    [Fact]
    public async Task Add_PersistsWorkoutAfterSaveChanges()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new WorkoutRepository(context);
        var workout = Workout.Log(Guid.NewGuid(), ExerciseType.Cardio, 30, 200, 5, 5, null, DateTime.UtcNow.AddDays(-1)).Value;

        repository.Add(workout);
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Workouts.CountAsync());
    }
}