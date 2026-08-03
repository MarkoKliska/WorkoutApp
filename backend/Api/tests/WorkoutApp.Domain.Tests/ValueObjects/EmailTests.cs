using WorkoutApp.Domain.Enums;
using WorkoutApp.Domain.ValueObjects;

namespace WorkoutApp.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("first.last@sub.example.com")]
    public void Create_WithValidEmail_ReturnsSuccess(string value)
    {
        var result = Email.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_TrimsAndLowercasesValue()
    {
        var result = Email.Create("  User@Example.COM  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ReturnsValidationError(string? value)
    {
        var result = Email.Create(value!);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.Empty", result.Error.Code);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    [InlineData("no-at-sign.com")]
    [InlineData("user@no-dot")]
    public void Create_WithInvalidFormat_ReturnsValidationError(string value)
    {
        var result = Email.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
    }

    [Fact]
    public void Equals_SameValueDifferentCase_AreEqual()
    {
        var first = Email.Create("user@example.com").Value;
        var second = Email.Create("USER@EXAMPLE.COM").Value;

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void Equals_DifferentValue_AreNotEqual()
    {
        var first = Email.Create("user1@example.com").Value;
        var second = Email.Create("user2@example.com").Value;

        Assert.NotEqual(first, second);
    }
}