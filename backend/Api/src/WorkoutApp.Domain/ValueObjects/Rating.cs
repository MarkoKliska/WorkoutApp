using WorkoutApp.Domain.Common;

namespace WorkoutApp.Domain.ValueObjects;

public sealed class Rating : ValueObject
{
    public int Value { get; }

    private Rating(int value)
    {
        Value = value;
    }

    public static Result<Rating> Create(int value)
    {
        if (value is < 1 or > 10)
            return Result.Failure<Rating>(Error.Validation("Rating.OutOfRange", "Rating must be between 1 and 10."));

        return Result.Success(new Rating(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}