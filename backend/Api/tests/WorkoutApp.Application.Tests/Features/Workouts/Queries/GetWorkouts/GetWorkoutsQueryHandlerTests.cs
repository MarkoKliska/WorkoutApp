using WorkoutApp.Application.Features.Workouts.Queries.GetWorkouts;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.Tests.Features.Workouts.Queries.GetWorkouts;

public class GetWorkoutsQueryHandlerTests
{
    private readonly FakeWorkoutRepository _workoutRepository = new();
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeCurrentUserService _currentUserService = new();

    private GetWorkoutsQueryHandler CreateHandler() => new(_workoutRepository, _userRepository, _currentUserService);

    private Guid SeedUser()
    {
        var user = User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;
        _userRepository.Seed(user);
        _currentUserService.UserId = user.Id;
        return user.Id;
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService.UserId = null;

        var result = await CreateHandler().Handle(new GetWorkoutsQuery(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        _currentUserService.UserId = Guid.NewGuid();

        var result = await CreateHandler().Handle(new GetWorkoutsQuery(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithNoWorkouts_ReturnsEmptyList()
    {
        SeedUser();

        var result = await CreateHandler().Handle(new GetWorkoutsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ReturnsOnlyCurrentUsersWorkoutsMappedToResponse()
    {
        var userId = SeedUser();
        var performedAt = DateTime.UtcNow.AddDays(-2);
        var ownWorkout = Workout.Log(userId, ExerciseType.Strength, 40, 300, 7, 6, "Leg day.", performedAt).Value;
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