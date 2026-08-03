using WorkoutApp.Application.DTOs.User.RegisterUser;
using WorkoutApp.Application.Features.Users.Commands.Register;

namespace WorkoutApp.Application.Tests.Features.Users.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    private static RegisterRequest ValidRequest() => new("John", "Doe", "john.doe@example.com", "Password1");

    private static RegisterCommand CommandWith(Func<RegisterRequest, RegisterRequest> mutate) =>
        new(mutate(ValidRequest()));

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var result = _validator.Validate(CommandWith(r => r));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyFirstName_HasErrorForFirstName(string firstName)
    {
        var result = _validator.Validate(CommandWith(r => r with { FirstName = firstName }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.FirstName");
    }

    [Fact]
    public void Validate_WithFirstNameExceedingMaxLength_HasErrorForFirstName()
    {
        var result = _validator.Validate(CommandWith(r => r with { FirstName = new string('a', 101) }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.FirstName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyLastName_HasErrorForLastName(string lastName)
    {
        var result = _validator.Validate(CommandWith(r => r with { LastName = lastName }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.LastName");
    }

    [Fact]
    public void Validate_WithInvalidEmailFormat_HasErrorForEmail()
    {
        var result = _validator.Validate(CommandWith(r => r with { Email = "not-an-email" }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Email");
    }

    [Fact]
    public void Validate_WithEmailExceedingMaxLength_HasErrorForEmail()
    {
        var longLocalPart = new string('a', 310);

        var result = _validator.Validate(CommandWith(r => r with { Email = $"{longLocalPart}@example.com" }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public void Validate_WithInvalidPassword_HasErrorForPassword(string password)
    {
        var result = _validator.Validate(CommandWith(r => r with { Password = password }));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Request.Password");
    }
}