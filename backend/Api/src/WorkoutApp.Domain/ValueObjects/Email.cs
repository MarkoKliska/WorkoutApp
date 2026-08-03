using System.Text.RegularExpressions;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>(Error.Validation("Email.Empty", "Email is required."));

        var trimmedValue = value.Trim();

        if (!Pattern.IsMatch(trimmedValue))
            return Result.Failure<Email>(Error.Validation("Email.InvalidFormat", "Email format is invalid."));

        return Result.Success(new Email(trimmedValue.ToLowerInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}