using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Domain.Tests.Common;

public class ErrorTests
{
    [Fact]
    public void None_HasEmptyCodeAndMessage()
    {
        var error = Error.None;

        Assert.Equal(string.Empty, error.Code);
        Assert.Equal(string.Empty, error.Message);
        Assert.Equal(ErrorType.Failure, error.Type);
    }

    [Theory]
    [InlineData(ErrorType.Failure)]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Unauthorized)]
    public void FactoryMethods_CreateErrorWithExpectedType(ErrorType expectedType)
    {
        var error = expectedType switch
        {
            ErrorType.Failure => Error.Failure("code", "message"),
            ErrorType.Validation => Error.Validation("code", "message"),
            ErrorType.NotFound => Error.NotFound("code", "message"),
            ErrorType.Conflict => Error.Conflict("code", "message"),
            ErrorType.Unauthorized => Error.Unauthorized("code", "message"),
            _ => throw new ArgumentOutOfRangeException(nameof(expectedType))
        };

        Assert.Equal("code", error.Code);
        Assert.Equal("message", error.Message);
        Assert.Equal(expectedType, error.Type);
    }

    [Fact]
    public void Equals_SameCodeMessageAndType_ReturnsTrue()
    {
        var first = Error.Validation("Test.Code", "Test message.");
        var second = Error.Validation("Test.Code", "Test message.");

        Assert.Equal(first, second);
    }
}