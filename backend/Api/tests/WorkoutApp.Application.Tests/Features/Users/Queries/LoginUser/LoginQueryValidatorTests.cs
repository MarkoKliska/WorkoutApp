using WorkoutApp.Application.DTOs.User.LoginUser;
using WorkoutApp.Application.Features.Users.Queries.Login;

namespace WorkoutApp.Application.Tests.Features.Users.Queries.LoginUser;

public class LoginQueryValidatorTests
{
    private readonly LoginQueryValidator _validator = new();

    private static LoginRequest ValidRequest() => new("john.doe@example.com", "Password1");

    private static LoginQuery QueryWith(Func<LoginRequest, LoginRequest> mutate) =>
        new(mutate(ValidRequest()));

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var result = _validator.Validate(QueryWith(r => r));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_HasErrorForEmail(string email)
    {
        var result = _validator.Validate(QueryWith(r => r with { Email = email }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Email");
    }

    [Fact]
    public void Validate_WithEmptyPassword_HasErrorForPassword()
    {
        var result = _validator.Validate(QueryWith(r => r with { Password = "" }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Password");
    }
}