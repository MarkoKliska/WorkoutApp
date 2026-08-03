using WorkoutApp.Application.DTOs.User.DeleteAccount;
using WorkoutApp.Application.Features.Users.Commands.DeleteAccount;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Application.Tests.Features.Users.Commands.DeleteAccount;

public class DeleteAccountCommandHandlerTests
{
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeWorkoutRepository _workoutRepository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakePasswordHasher _passwordHasher = new();
    private readonly FakeCurrentUserService _currentUserService = new();

    private DeleteAccountCommandHandler CreateHandler() =>
        new(_userRepository, _workoutRepository, _unitOfWork, _passwordHasher, _currentUserService);

    private User SeedUser(string password)
    {
        var user = User.Register("John", "Doe", "john.doe@example.com", _passwordHasher.Hash(password)).Value;
        _userRepository.Seed(user);
        _currentUserService.UserId = user.Id;
        return user;
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService.UserId = null;
        var command = new DeleteAccountCommand(new DeleteAccountRequest("Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        _currentUserService.UserId = Guid.NewGuid();
        var command = new DeleteAccountCommand(new DeleteAccountRequest("Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithIncorrectPassword_ReturnsUnauthorizedAndDoesNotDelete()
    {
        var user = SeedUser("Password1");
        var command = new DeleteAccountCommand(new DeleteAccountRequest("WrongPassword"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidPassword", result.Error.Code);
        Assert.False(user.IsDeleted);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithCorrectPassword_DeletesUserAndReturnsSuccess()
    {
        var user = SeedUser("Password1");
        var command = new DeleteAccountCommand(new DeleteAccountRequest("Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(user.IsDeleted);
        Assert.NotNull(user.DeletedAt);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithCorrectPassword_AlsoSoftDeletesUsersWorkouts()
    {
        var user = SeedUser("Password1");
        var workout1 = Workout.Log(user.Id, ExerciseType.Cardio, 30, 200, 5, 5, null, DateTime.UtcNow.AddDays(-1)).Value;
        var workout2 = Workout.Log(user.Id, ExerciseType.Strength, 40, 300, 6, 6, null, DateTime.UtcNow.AddDays(-2)).Value;
        _workoutRepository.Seed(workout1);
        _workoutRepository.Seed(workout2);
        var command = new DeleteAccountCommand(new DeleteAccountRequest("Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(workout1.IsDeleted);
        Assert.True(workout2.IsDeleted);
    }
}