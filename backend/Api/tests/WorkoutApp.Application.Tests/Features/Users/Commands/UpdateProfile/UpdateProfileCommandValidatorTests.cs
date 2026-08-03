using WorkoutApp.Application.DTOs.User.UpdateProfile;
using WorkoutApp.Application.Features.Users.Commands.UpdateProfile;

namespace WorkoutApp.Application.Tests.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandValidatorTests
{
    private readonly UpdateProfileCommandValidator _validator = new();

    private static UpdateProfileRequest ValidRequest() => new("John", "Doe", "john.doe@example.com");

    private static UpdateProfileCommand CommandWith(Func<UpdateProfileRequest, UpdateProfileRequest> mutate) =>
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
}