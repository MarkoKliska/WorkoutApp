using WorkoutApp.Application.DTOs.User.LoginUser;
using WorkoutApp.Application.Features.Users.Queries.Login;
using WorkoutApp.Application.Tests.TestDoubles;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Application.Tests.Features.Users.Queries.LoginUser;

public class LoginQueryHandlerTests
{
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakePasswordHasher _passwordHasher = new();
    private readonly FakeTokenService _tokenService = new();

    private LoginQueryHandler CreateHandler() => new(_userRepository, _passwordHasher, _tokenService);

    [Fact]
    public async Task Handle_WithUnknownEmail_ReturnsInvalidCredentials()
    {
        var command = new LoginQuery(new LoginRequest("nobody@example.com", "Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithIncorrectPassword_ReturnsInvalidCredentials()
    {
        var user = User.Register("John", "Doe", "john.doe@example.com", _passwordHasher.Hash("Password1")).Value;
        _userRepository.Seed(user);
        var command = new LoginQuery(new LoginRequest("john.doe@example.com", "WrongPassword"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidCredentials", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokenAndUserId()
    {
        var user = User.Register("John", "Doe", "john.doe@example.com", _passwordHasher.Hash("Password1")).Value;
        _userRepository.Seed(user);
        var command = new LoginQuery(new LoginRequest("john.doe@example.com", "Password1"));

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal($"token-for-{user.Id}", result.Value.Token);
    }
}