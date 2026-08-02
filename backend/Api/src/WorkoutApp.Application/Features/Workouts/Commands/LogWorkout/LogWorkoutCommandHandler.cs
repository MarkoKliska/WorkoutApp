using MediatR;
using WorkoutApp.Application.DTOs.Workout.LogWorkout;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Workouts.Commands.LogWorkout;

public sealed class LogWorkoutCommandHandler(
    IWorkoutRepository workoutRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService
) 
    : IRequestHandler<LogWorkoutCommand, Result<LogWorkoutResponse>>
{
    public async Task<Result<LogWorkoutResponse>> Handle(LogWorkoutCommand command, CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not { } userId)
            return Result.Failure<LogWorkoutResponse>(
                Error.Unauthorized("Workout.Unauthorized", "You must be logged in to log a workout."));

        var request = command.Request;

        var workoutResult = Workout.Log(
            userId, request.ExerciseType, request.DurationMinutes, request.CaloriesBurned,
            request.Difficulty, request.Fatigue, request.Notes, request.PerformedAt);

        if (workoutResult.IsFailure)
            return Result.Failure<LogWorkoutResponse>(workoutResult.Error);

        var workout = workoutResult.Value;
        workoutRepository.Add(workout);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LogWorkoutResponse(workout.Id));
    }
}