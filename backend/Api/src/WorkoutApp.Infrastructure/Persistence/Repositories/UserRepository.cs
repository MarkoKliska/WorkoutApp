using Microsoft.EntityFrameworkCore;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Repositories;
using WorkoutApp.Domain.ValueObjects;
using WorkoutApp.Infrastructure.Persistence.Contexts;

namespace WorkoutApp.Infrastructure.Persistence.Repositories;

public class UserRepository(
    WorkoutAppDbContext context
) : IUserRepository
{
    public async Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null
            ? Result.Failure<User>(Error.NotFound("User.NotFound", "User was not found."))
            : Result.Success(user);
    }

    public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == emailResult.Value, cancellationToken);

        return user is null
            ? Result.Failure<User>(Error.NotFound("User.NotFound", "User was not found."))
            : Result.Success(user);
    }

    public void Add(User user) => context.Users.Add(user);
}