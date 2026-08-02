using MediatR;
using WorkoutApp.Application.DTOs.Workout;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Workouts.Queries.GetWorkoutById;

public sealed record GetWorkoutByIdQuery(Guid WorkoutId) 
    : IRequest<Result<WorkoutResponse>>;