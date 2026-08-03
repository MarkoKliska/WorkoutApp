using WorkoutApp.Domain.ValueObjects;

namespace WorkoutApp.Domain.Tests.ValueObjects;

public class RatingTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_WithValueInRange_ReturnsSuccess(int value)
    {
        var result = Rating.Create(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(100)]
    public void Create_WithValueOutOfRange_ReturnsValidationError(int value)
    {
        var result = Rating.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal("Rating.OutOfRange", result.Error.Code);
    }

    [Fact]
    public void Equals_SameValue_AreEqual()
    {
        var first = Rating.Create(5).Value;
        var second = Rating.Create(5).Value;

        Assert.Equal(first, second);
    }
}