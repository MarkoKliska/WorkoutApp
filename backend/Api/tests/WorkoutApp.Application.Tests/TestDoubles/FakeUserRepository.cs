using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public IReadOnlyList<User> Users => _users;

    public Error? ForcedGetByEmailFailure { get; set; }

    public void Seed(User user) => _users.Add(user);

    public Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
        return Task.FromResult(user is null
            ? Result.Failure<User>(Error.NotFound("User.NotFound", "User was not found."))
            : Result.Success(user));
    }

    public Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (ForcedGetByEmailFailure is { } forcedError)
            return Task.FromResult(Result.Failure<User>(forcedError));

        var user = _users.FirstOrDefault(u =>
            !u.IsDeleted && string.Equals(u.Email.Value, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user is null
            ? Result.Failure<User>(Error.NotFound("User.NotFound", "User was not found."))
            : Result.Success(user));
    }

    public void Add(User user) => _users.Add(user);
}