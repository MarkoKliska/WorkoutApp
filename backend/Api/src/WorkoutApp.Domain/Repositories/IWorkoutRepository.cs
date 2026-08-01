using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Domain.Repositories;

public interface IWorkoutRepository
{
    Task<Result<Workout>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Workout>> GetByUserAndDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    void Add(Workout workout);
}