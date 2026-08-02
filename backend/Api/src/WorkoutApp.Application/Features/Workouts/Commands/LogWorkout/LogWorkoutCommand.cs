using MediatR;
using WorkoutApp.Application.DTOs.Workout.LogWorkout;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Workouts.Commands.LogWorkout;

public sealed record LogWorkoutCommand(LogWorkoutRequest Request) 
    : IRequest<Result<LogWorkoutResponse>>;