using WorkoutApp.Domain.Common;

namespace WorkoutApp.Domain.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessfulResultWithNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ReturnsFailedResultWithGivenError()
    {
        var error = Error.Failure("Test.Error", "Something went wrong.");

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void SuccessOfT_ReturnsSuccessfulResultExposingValue()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void FailureOfT_AccessingValue_ThrowsInvalidOperationException()
    {
        var result = Result.Failure<int>(Error.Failure("Test.Error", "Something went wrong."));

        Assert.True(result.IsFailure);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromNonNullValue_ReturnsSuccessResult()
    {
        Result<string> result = "hello";

        Assert.True(result.IsSuccess);
        Assert.Equal("hello", result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromNullValue_ReturnsFailureResult()
    {
        string? value = null;

        Result<string> result = value!;

        Assert.True(result.IsFailure);
        Assert.Equal("Result.NullValue", result.Error.Code);
    }
}