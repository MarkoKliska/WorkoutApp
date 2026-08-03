using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakeWorkoutRepository : IWorkoutRepository
{
    private readonly List<Workout> _workouts = [];

    public IReadOnlyList<Workout> Workouts => _workouts;

    public void Seed(Workout workout) => _workouts.Add(workout);

    public Task<Result<Workout>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workout = _workouts.FirstOrDefault(w => w.Id == id && !w.IsDeleted);
        return Task.FromResult(workout is null
            ? Result.Failure<Workout>(Error.NotFound("Workout.NotFound", "Workout was not found."))
            : Result.Success(workout));
    }

    public Task<IReadOnlyList<Workout>> GetByUserAndDateRangeAsync(
        Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Workout> result = _workouts
            .Where(w => !w.IsDeleted && w.UserId == userId && w.PerformedAt >= from && w.PerformedAt <= to)
            .ToList();
        return Task.FromResult(result);
    }

    public void Add(Workout workout) => _workouts.Add(workout);

    public Task<IReadOnlyList<Workout>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Workout> result = _workouts
            .Where(w => !w.IsDeleted && w.UserId == userId)
            .ToList();
        return Task.FromResult(result);
    }
}