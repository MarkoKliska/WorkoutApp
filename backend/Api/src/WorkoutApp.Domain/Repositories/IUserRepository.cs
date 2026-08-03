using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Domain.Repositories;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
}
