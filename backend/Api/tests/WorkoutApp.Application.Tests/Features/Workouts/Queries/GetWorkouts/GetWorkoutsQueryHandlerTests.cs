using WorkoutApp.Application.Features.Workouts.Queries.GetWorkouts;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.Tests.Features.Workouts.Queries.GetWorkouts;

public class GetWorkoutsQueryHandlerTests
{
    private readonly FakeWorkoutRepository _workoutRepository = new();
    private readonly FakeCurrentUserService _currentUserService = new();
    private readonly Guid _userId = Guid.NewGuid();

    public GetWorkoutsQueryHandlerTests()
    {
        _currentUserService.UserId = _userId;
    }

    private GetWorkoutsQueryHandler CreateHandler() => new(_workoutRepository, _currentUserService);

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService.UserId = null;

        var result = await CreateHandler().Handle(new GetWorkoutsQuery(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithNoWorkouts_ReturnsEmptyList()
    {
        var result = await CreateHandler().Handle(new GetWorkoutsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ReturnsOnlyCurrentUsersWorkoutsMappedToResponse()
    {
        var performedAt = DateTime.UtcNow.AddDays(-2);
        var ownWorkout = Workout.Log(_userId, ExerciseType.Strength, 40, 300, 7, 6, "Leg day.", performedAt).Value;
        var otherUsersWorkout = Workout.Log(Guid.NewGuid(), ExerciseType.Cardio, 20, 150, 3, 2, null, performedAt).Value;
        _workoutRepository.Seed(ownWorkout);
        _workoutRepository.Seed(otherUsersWorkout);

        var result = await CreateHandler().Handle(new GetWorkoutsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var response = Assert.Single(result.Value);
        Assert.Equal(ownWorkout.Id, response.Id);
        Assert.Equal(ExerciseType.Strength, response.ExerciseType);
        Assert.Equal(40, response.DurationMinutes);
        Assert.Equal(300, response.CaloriesBurned);
        Assert.Equal(7, response.Difficulty);
        Assert.Equal(6, response.Fatigue);
        Assert.Equal("Leg day.", response.Notes);
        Assert.Equal(performedAt, response.PerformedAt);
    }
}