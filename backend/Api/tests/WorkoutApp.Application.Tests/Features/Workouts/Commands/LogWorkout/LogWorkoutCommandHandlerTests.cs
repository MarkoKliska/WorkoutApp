using WorkoutApp.Application.DTOs.Workout.LogWorkout;
using WorkoutApp.Application.Features.Workouts.Commands.LogWorkout;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.Tests.Features.Workouts.Commands.LogWorkout;

public class LogWorkoutCommandHandlerTests
{
    private readonly FakeWorkoutRepository _workoutRepository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakeCurrentUserService _currentUserService = new();

    private LogWorkoutCommandHandler CreateHandler() =>
        new(_workoutRepository, _unitOfWork, _currentUserService);

    private static LogWorkoutRequest ValidRequest() => new(
        ExerciseType.Cardio, 30, 200, 5, 5, "Felt good.", DateTime.UtcNow.AddDays(-1));

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService.UserId = null;
        var command = new LogWorkoutCommand(ValidRequest());

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.Unauthorized", result.Error.Code);
        Assert.Empty(_workoutRepository.Workouts);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CreatesWorkoutAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        _currentUserService.UserId = userId;
        var command = new LogWorkoutCommand(ValidRequest());

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(_workoutRepository.Workouts);
        var workout = _workoutRepository.Workouts[0];
        Assert.Equal(userId, workout.UserId);
        Assert.Equal(workout.Id, result.Value.WorkoutId);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithNonPositiveDuration_ReturnsFailureAndDoesNotPersist()
    {
        _currentUserService.UserId = Guid.NewGuid();
        var command = new LogWorkoutCommand(ValidRequest() with { DurationMinutes = 0 });

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.InvalidDuration", result.Error.Code);
        Assert.Empty(_workoutRepository.Workouts);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithDifficultyOutOfRange_ReturnsFailureAndDoesNotPersist()
    {
        _currentUserService.UserId = Guid.NewGuid();
        var command = new LogWorkoutCommand(ValidRequest() with { Difficulty = 0 });

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Rating.OutOfRange", result.Error.Code);
        Assert.Empty(_workoutRepository.Workouts);
    }
}