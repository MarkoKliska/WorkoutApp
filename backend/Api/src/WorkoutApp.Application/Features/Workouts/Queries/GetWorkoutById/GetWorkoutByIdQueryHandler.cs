using MediatR;
using WorkoutApp.Application.DTOs.Workout;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Workouts.Queries.GetWorkoutById;

public sealed class GetWorkoutByIdQueryHandler(
    IWorkoutRepository workoutRepository,
    ICurrentUserService currentUserService
)
    : IRequestHandler<GetWorkoutByIdQuery, Result<WorkoutResponse>>
{
    public async Task<Result<WorkoutResponse>> Handle(GetWorkoutByIdQuery query, CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not { } userId)
            return Result.Failure<WorkoutResponse>(
                Error.Unauthorized("Workout.Unauthorized", "You must be logged in to view a workout."));

        var workoutResult = await workoutRepository.GetByIdAsync(query.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
            return Result.Failure<WorkoutResponse>(workoutResult.Error);

        var workout = workoutResult.Value;
        if (workout.UserId != userId)
            return Result.Failure<WorkoutResponse>(
                Error.NotFound("Workout.NotFound", $"Workout with Id '{query.WorkoutId}' was not found."));

        return Result.Success(new WorkoutResponse(
            workout.Id, workout.ExerciseType, workout.DurationMinutes, workout.CaloriesBurned,
            workout.Difficulty.Value, workout.Fatigue.Value, workout.Notes, workout.PerformedAt));
    }
}