using WorkoutApp.Application.Features.Workouts.Queries.GetMonthlyProgress;

namespace WorkoutApp.Application.Tests.Features.Workouts.Queries.GetMonthlyProgress;

public class GetMonthlyProgressQueryValidatorTests
{
    private readonly GetMonthlyProgressQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidYearAndMonth_HasNoErrors()
    {
        var result = _validator.Validate(new GetMonthlyProgressQuery(2020, 6));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Validate_WithMonthOutOfRange_HasErrorForMonth(int month)
    {
        var result = _validator.Validate(new GetMonthlyProgressQuery(2020, month));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Month");
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(2101)]
    public void Validate_WithYearOutOfRange_HasErrorForYear(int year)
    {
        var result = _validator.Validate(new GetMonthlyProgressQuery(year, 6));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Year");
    }
}