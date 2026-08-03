using WorkoutApp.Domain.Common;

namespace WorkoutApp.Domain.ValueObjects;

public sealed class PersonName : ValueObject
{
    private const int MaxLength = 100;

    public string Value { get; }

    private PersonName(string value)
    {
        Value = value;
    }

    public static Result<PersonName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PersonName>(Error.Validation("PersonName.Empty", "Name is required."));

        if (value.Length > MaxLength)
            return Result.Failure<PersonName>(Error.Validation("PersonName.TooLong", $"Name cannot exceed {MaxLength} characters."));

        return Result.Success(new PersonName(value.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}