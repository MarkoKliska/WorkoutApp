using WorkoutApp.Domain.ValueObjects;

namespace WorkoutApp.Domain.Tests.ValueObjects;

public class PersonNameTests
{
    [Fact]
    public void Create_WithValidValue_ReturnsSuccess()
    {
        var result = PersonName.Create("John");

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Value.Value);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var result = PersonName.Create("  John  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ReturnsValidationError(string? value)
    {
        var result = PersonName.Create(value!);

        Assert.True(result.IsFailure);
        Assert.Equal("PersonName.Empty", result.Error.Code);
    }

    [Fact]
    public void Create_AtMaxLength_ReturnsSuccess()
    {
        var value = new string('a', 100);

        var result = PersonName.Create(value);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_ExceedingMaxLength_ReturnsValidationError()
    {
        var value = new string('a', 101);

        var result = PersonName.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal("PersonName.TooLong", result.Error.Code);
    }

    [Fact]
    public void Equals_SameValue_AreEqual()
    {
        var first = PersonName.Create("John").Value;
        var second = PersonName.Create("John").Value;

        Assert.Equal(first, second);
    }
}