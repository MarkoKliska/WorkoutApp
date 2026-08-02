using Microsoft.EntityFrameworkCore;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Repositories;
using WorkoutApp.Infrastructure.Persistence.Contexts;

namespace WorkoutApp.Infrastructure.Persistence.Repositories;

public class WorkoutRepository(
    WorkoutAppDbContext context
) : IWorkoutRepository
{
    public async Task<Result<Workout>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workout = await context.Workouts.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return workout is null
            ? Result.Failure<Workout>(Error.NotFound("Workout.NotFound", $"Workout with Id '{id}' was not found."))
            : Result.Success(workout);
    }

    public async Task<IReadOnlyList<Workout>> GetByUserAndDateRangeAsync(
        Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await context.Workouts
            .Where(w => w.UserId == userId && w.PerformedAt >= from && w.PerformedAt <= to)
            .OrderBy(w => w.PerformedAt)
            .ToListAsync(cancellationToken);
    }

    public void Add(Workout workout) => context.Workouts.Add(workout);
}