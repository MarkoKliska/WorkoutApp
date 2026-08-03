using MediatR;
using WorkoutApp.Application.DTOs.Workout.GetMonthlyProgress;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Workouts.Queries.GetMonthlyProgress;

public sealed record GetMonthlyProgressQuery(int Year, int Month) 
    : IRequest<Result<MonthlyProgressResponse>>;