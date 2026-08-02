using MediatR;
using WorkoutApp.Application.DTOs.Workout;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Workouts.Queries.GetWorkouts;

public sealed class GetWorkoutsQueryHandler(
    IWorkoutRepository workoutRepository,
    ICurrentUserService currentUserService
) 
    : IRequestHandler<GetWorkoutsQuery, Result<IReadOnlyList<WorkoutResponse>>>
{
    public async Task<Result<IReadOnlyList<WorkoutResponse>>> Handle(GetWorkoutsQuery query, CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not { } userId)
            return Result.Failure<IReadOnlyList<WorkoutResponse>>(
                Error.Unauthorized("Workout.Unauthorized", "You must be logged in to view workouts."));

        var workouts = await workoutRepository.GetByUserAsync(userId, cancellationToken);

        var response = workouts
            .Select(w => new WorkoutResponse(
                w.Id, w.ExerciseType, w.DurationMinutes, w.CaloriesBurned,
                w.Difficulty.Value, w.Fatigue.Value, w.Notes, w.PerformedAt))
            .ToList();

        return Result.Success<IReadOnlyList<WorkoutResponse>>(response);
    }
}