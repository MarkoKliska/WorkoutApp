using WorkoutApp.Domain.Repositories;
using WorkoutApp.Infrastructure.Persistence.Contexts;

namespace WorkoutApp.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork(
    WorkoutAppDbContext context
) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}