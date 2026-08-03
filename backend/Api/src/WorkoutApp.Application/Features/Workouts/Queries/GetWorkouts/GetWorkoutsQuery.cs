using MediatR;
using WorkoutApp.Application.DTOs.Workout;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Workouts.Queries.GetWorkouts;

public sealed record GetWorkoutsQuery 
    : IRequest<Result<IReadOnlyList<WorkoutResponse>>>;