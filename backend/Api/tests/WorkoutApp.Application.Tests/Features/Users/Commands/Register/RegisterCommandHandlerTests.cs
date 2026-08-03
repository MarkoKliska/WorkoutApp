using Microsoft.Extensions.Logging;
using WorkoutApp.Application.DTOs.User.RegisterUser;
using WorkoutApp.Application.Features.Users.Commands.Register;
using WorkoutApp.Application.Features.Users.Events;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Application.Tests.Features.Users.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakePasswordHasher _passwordHasher = new();
    private readonly FakeTokenService _tokenService = new();
    private readonly FakePublisher _publisher = new();
    private readonly FakeLogger<RegisterCommandHandler> _logger = new();

    private RegisterCommandHandler CreateHandler() =>
        new(_userRepository, _unitOfWork, _passwordHasher, _tokenService, _publisher, _logger);

    [Fact]
    public async Task Handle_WithNewEmail_CreatesUserAndReturnsSuccess()
    {
        var command = new RegisterCommand(new RegisterRequest("John", "Doe", "john.doe@example.com", "Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(_userRepository.Users);
        var createdUser = _userRepository.Users[0];
        Assert.Equal(createdUser.Id, result.Value.UserId);
        Assert.Equal($"token-for-{createdUser.Id}", result.Value.Token);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
        var publishedNotification = Assert.Single(_publisher.PublishedNotifications);
        var userRegisteredEvent = Assert.IsType<UserRegisteredEvent>(publishedNotification);
        Assert.Equal(createdUser.Id, userRegisteredEvent.UserId);
        Assert.Equal("john.doe@example.com", userRegisteredEvent.Email);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ReturnsConflictAndDoesNotPersist()
    {
        var existingUser = User.Register("Existing", "User", "john.doe@example.com", "hashed").Value;
        _userRepository.Seed(existingUser);
        var command = new RegisterCommand(new RegisterRequest("John", "Doe", "john.doe@example.com", "Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.EmailAlreadyExists", result.Error.Code);
        Assert.Single(_userRepository.Users);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
        Assert.Empty(_publisher.PublishedNotifications);
    }

    [Fact]
    public async Task Handle_WithInvalidFirstName_ReturnsFailureAndDoesNotPersist()
    {
        var command = new RegisterCommand(new RegisterRequest("", "Doe", "john.doe@example.com", "Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("PersonName.Empty", result.Error.Code);
        Assert.Empty(_userRepository.Users);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenEmailLookupFailsUnexpectedly_PropagatesError()
    {
        _userRepository.ForcedGetByEmailFailure = Error.Failure("User.LookupFailed", "Lookup failed.");
        var command = new RegisterCommand(new RegisterRequest("John", "Doe", "john.doe@example.com", "Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.LookupFailed", result.Error.Code);
        Assert.Empty(_userRepository.Users);
    }

    [Fact]
    public async Task Handle_WhenPublishingEventFails_StillCreatesUserAndLogsError()
    {
        _publisher.ThrowOnPublish = new InvalidOperationException("Message bus unavailable.");
        var command = new RegisterCommand(new RegisterRequest("John", "Doe", "john.doe@example.com", "Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(_userRepository.Users);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
        var logEntry = Assert.Single(_logger.LogEntries);
        Assert.Equal(LogLevel.Error, logEntry.Level);
        Assert.IsType<InvalidOperationException>(logEntry.Exception);
    }
}