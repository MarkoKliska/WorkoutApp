using WorkoutApp.Application.Features.Users.Queries.GetProfile;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Application.Tests.Features.Users.Queries.GetProfile;

public class GetProfileQueryHandlerTests
{
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakeCurrentUserService _currentUserService = new();

    private GetProfileQueryHandler CreateHandler() => new(_userRepository, _currentUserService);

    [Fact]
    public async Task Handle_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUserService.UserId = null;

        var result = await CreateHandler().Handle(new GetProfileQuery(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.Unauthorized", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        _currentUserService.UserId = Guid.NewGuid();

        var result = await CreateHandler().Handle(new GetProfileQuery(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithAuthenticatedUser_ReturnsProfile()
    {
        var user = User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;
        _userRepository.Seed(user);
        _currentUserService.UserId = user.Id;

        var result = await CreateHandler().Handle(new GetProfileQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("john.doe@example.com", result.Value.Email);
    }
}