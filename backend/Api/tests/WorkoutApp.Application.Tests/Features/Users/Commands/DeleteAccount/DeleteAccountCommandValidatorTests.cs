using WorkoutApp.Application.DTOs.User.DeleteAccount;
using WorkoutApp.Application.Features.Users.Commands.DeleteAccount;

namespace WorkoutApp.Application.Tests.Features.Users.Commands.DeleteAccount;

public class DeleteAccountCommandValidatorTests
{
    private readonly DeleteAccountCommandValidator _validator = new();

    [Fact]
    public void Validate_WithNonEmptyPassword_HasNoErrors()
    {
        var result = _validator.Validate(new DeleteAccountCommand(new DeleteAccountRequest("secret")));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyPassword_HasErrorForPassword(string password)
    {
        var result = _validator.Validate(new DeleteAccountCommand(new DeleteAccountRequest(password)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Password");
    }
}