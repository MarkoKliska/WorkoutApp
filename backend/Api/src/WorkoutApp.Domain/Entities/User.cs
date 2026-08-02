using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.ValueObjects;

namespace WorkoutApp.Domain.Entities;

public sealed class User : AggregateRoot, ISoftDeletable
{
    public PersonName FirstName { get; private set; } = null!;
    public PersonName LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public string FullName => $"{FirstName.Value} {LastName.Value}";

    private User(Guid id, PersonName firstName, PersonName lastName, Email email, string passwordHash) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }

    private User() { }

    public static Result<User> Register(string firstName, string lastName, string email, string passwordHash)
    {
        var firstNameResult = PersonName.Create(firstName);
        if (firstNameResult.IsFailure)
            return Result.Failure<User>(firstNameResult.Error);

        var lastNameResult = PersonName.Create(lastName);
        if (lastNameResult.IsFailure)
            return Result.Failure<User>(lastNameResult.Error);

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);

        return Result.Success(new User(
            Guid.NewGuid(), firstNameResult.Value, lastNameResult.Value, emailResult.Value, passwordHash));
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
    public Result UpdateProfile(string firstName, string lastName, string email)
    {
        var firstNameResult = PersonName.Create(firstName);
        if (firstNameResult.IsFailure)
            return Result.Failure(firstNameResult.Error);

        var lastNameResult = PersonName.Create(lastName);
        if (lastNameResult.IsFailure)
            return Result.Failure(lastNameResult.Error);

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        FirstName = firstNameResult.Value;
        LastName = lastNameResult.Value;
        Email = emailResult.Value;

        return Result.Success();
    }
}