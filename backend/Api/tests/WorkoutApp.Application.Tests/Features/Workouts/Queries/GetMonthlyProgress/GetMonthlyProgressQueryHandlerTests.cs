using WorkoutApp.Application.Features.Workouts.Queries.GetMonthlyProgress;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.Tests.Features.Workouts.Queries.GetMonthlyProgress;

public class GetMonthlyProgressQueryHandlerTests
{
    private readonly FakeWorkoutRepository _workoutRepository = new();
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeCurrentUserService _currentUserService = new();
    private readonly Guid _userId;

    public GetMonthlyProgressQueryHandlerTests()
    {
        var user = User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;
        _userRepository.Seed(user);
        _userId = user.Id;
        _currentUserService.UserId = _userId;
    }

    private GetMonthlyProgressQueryHandler CreateHandler() =>
        new(_workoutRepository, _userRepository, _currentUserService);

    private void SeedWorkout(DateTime performedAt, int duration = 30, int difficulty = 5, int fatigue = 5, Guid? userId = null)
    {
        var workout = Workout.Log(
            userId ?? _userId, ExerciseType.Cardio, duration, 200, difficulty, fatigue, null, performedAt).Value;
        _workoutRepository.Seed(workout);
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService.UserId = null;

        var result = await CreateHandler().Handle(new GetMonthlyProgressQuery(2020, 1), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Workout.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        _currentUserService.UserId = Guid.NewGuid();

        var result = await CreateHandler().Handle(new GetMonthlyProgressQuery(2020, 1), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithNoWorkouts_ReturnsWeeksSpanningWholeMonthWithZeroedStats()
    {
        var result = await CreateHandler().Handle(new GetMonthlyProgressQuery(2020, 1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2020, result.Value.Year);
        Assert.Equal(1, result.Value.Month);
        Assert.Equal(new DateOnly(2020, 1, 1), result.Value.Weeks.First().WeekStart);
        Assert.Equal(new DateOnly(2020, 1, 31), result.Value.Weeks.Last().WeekEnd);
        Assert.All(result.Value.Weeks, week =>
        {
            Assert.Equal(0, week.WorkoutCount);
            Assert.Equal(0, week.TotalDurationMinutes);
            Assert.Equal(0.0, week.AverageDifficulty);
            Assert.Equal(0.0, week.AverageFatigue);
        });
    }

    [Fact]
    public async Task Handle_WithWorkoutsOnFirstAndLastDayOfMonth_PlacesEachInCorrectWeek()
    {
        SeedWorkout(new DateTime(2020, 1, 1, 6, 0, 0, DateTimeKind.Utc), duration: 45, difficulty: 6, fatigue: 4);
        SeedWorkout(new DateTime(2020, 1, 31, 18, 0, 0, DateTimeKind.Utc), duration: 60, difficulty: 8, fatigue: 7);

        var result = await CreateHandler().Handle(new GetMonthlyProgressQuery(2020, 1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var firstWeek = result.Value.Weeks.First();
        Assert.Equal(new DateOnly(2020, 1, 1), firstWeek.WeekStart);
        Assert.Equal(1, firstWeek.WorkoutCount);
        Assert.Equal(45, firstWeek.TotalDurationMinutes);
        Assert.Equal(6.0, firstWeek.AverageDifficulty);
        Assert.Equal(4.0, firstWeek.AverageFatigue);

        var lastWeek = result.Value.Weeks.Last();
        Assert.Equal(new DateOnly(2020, 1, 31), lastWeek.WeekEnd);
        Assert.Equal(1, lastWeek.WorkoutCount);
        Assert.Equal(60, lastWeek.TotalDurationMinutes);
        Assert.Equal(8.0, lastWeek.AverageDifficulty);
        Assert.Equal(7.0, lastWeek.AverageFatigue);
    }

    [Fact]
    public async Task Handle_WithMultipleWorkoutsInSameWeek_ComputesRoundedAverages()
    {
        SeedWorkout(new DateTime(2020, 1, 1, 6, 0, 0, DateTimeKind.Utc), duration: 30, difficulty: 5, fatigue: 3);
        SeedWorkout(new DateTime(2020, 1, 1, 18, 0, 0, DateTimeKind.Utc), duration: 20, difficulty: 6, fatigue: 6);

        var result = await CreateHandler().Handle(new GetMonthlyProgressQuery(2020, 1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var firstWeek = result.Value.Weeks.First();
        Assert.Equal(2, firstWeek.WorkoutCount);
        Assert.Equal(50, firstWeek.TotalDurationMinutes);
        Assert.Equal(5.5, firstWeek.AverageDifficulty);
        Assert.Equal(4.5, firstWeek.AverageFatigue);
    }

    [Fact]
    public async Task Handle_ExcludesWorkoutsOutsideRequestedMonthOrUser()
    {
        SeedWorkout(new DateTime(2020, 1, 15, 6, 0, 0, DateTimeKind.Utc), duration: 30);
        SeedWorkout(new DateTime(2020, 2, 1, 6, 0, 0, DateTimeKind.Utc), duration: 999);
        SeedWorkout(new DateTime(2020, 1, 15, 6, 0, 0, DateTimeKind.Utc), duration: 999, userId: Guid.NewGuid());

        var result = await CreateHandler().Handle(new GetMonthlyProgressQuery(2020, 1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Weeks.Sum(w => w.WorkoutCount));
        Assert.Equal(30, result.Value.Weeks.Sum(w => w.TotalDurationMinutes));
    }
}