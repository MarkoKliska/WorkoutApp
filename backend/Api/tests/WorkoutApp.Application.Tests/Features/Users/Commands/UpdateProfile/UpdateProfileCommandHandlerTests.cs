using WorkoutApp.Application.DTOs.User.UpdateProfile;
using WorkoutApp.Application.Features.Users.Commands.UpdateProfile;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Application.Tests.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandlerTests
{
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakeCurrentUserService _currentUserService = new();

    private UpdateProfileCommandHandler CreateHandler() =>
        new(_userRepository, _unitOfWork, _currentUserService);

    private User SeedUser(string email = "john.doe@example.com")
    {
        var user = User.Register("John", "Doe", email, "hashed").Value;
        _userRepository.Seed(user);
        _currentUserService.UserId = user.Id;
        return user;
    }

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService.UserId = null;
        var command = new UpdateProfileCommand(new UpdateProfileRequest("Jane", "Smith", "jane.smith@example.com"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        _currentUserService.UserId = Guid.NewGuid();
        var command = new UpdateProfileCommand(new UpdateProfileRequest("Jane", "Smith", "jane.smith@example.com"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithNewUniqueEmail_UpdatesSuccessfully()
    {
        SeedUser();
        var command = new UpdateProfileCommand(new UpdateProfileRequest("Jane", "Smith", "jane.smith@example.com"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", result.Value.FirstName);
        Assert.Equal("Smith", result.Value.LastName);
        Assert.Equal("jane.smith@example.com", result.Value.Email);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithSameEmailDifferentCase_SkipsConflictCheckAndSucceeds()
    {
        SeedUser("john.doe@example.com");
        var command = new UpdateProfileCommand(new UpdateProfileRequest("John", "Doe", "JOHN.DOE@EXAMPLE.COM"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("john.doe@example.com", result.Value.Email);
    }

    [Fact]
    public async Task Handle_WithEmailAlreadyTakenByAnotherUser_ReturnsConflictAndDoesNotPersist()
    {
        var user = SeedUser("john.doe@example.com");
        var otherUser = User.Register("Jane", "Smith", "jane.smith@example.com", "hashed").Value;
        _userRepository.Seed(otherUser);
        var command = new UpdateProfileCommand(new UpdateProfileRequest("John", "Doe", "jane.smith@example.com"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.EmailAlreadyExists", result.Error.Code);
        Assert.Equal("john.doe@example.com", user.Email.Value);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WithInvalidEmailFormat_ReturnsFailureAndDoesNotPersist()
    {
        SeedUser();
        var command = new UpdateProfileCommand(new UpdateProfileRequest("Jane", "Smith", "not-an-email"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenEmailLookupFailsUnexpectedly_PropagatesError()
    {
        SeedUser();
        _userRepository.ForcedGetByEmailFailure = Error.Failure("User.LookupFailed", "Lookup failed.");
        var command = new UpdateProfileCommand(new UpdateProfileRequest("Jane", "Smith", "jane.smith@example.com"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.LookupFailed", result.Error.Code);
    }
}