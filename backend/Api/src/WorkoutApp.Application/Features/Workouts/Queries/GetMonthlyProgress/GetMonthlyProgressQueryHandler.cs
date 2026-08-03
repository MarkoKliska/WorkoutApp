using MediatR;
using WorkoutApp.Application.DTOs.Workout.GetMonthlyProgress;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Workouts.Queries.GetMonthlyProgress;

public sealed class GetMonthlyProgressQueryHandler(
    IWorkoutRepository workoutRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetMonthlyProgressQuery, Result<MonthlyProgressResponse>>
{
    public async Task<Result<MonthlyProgressResponse>> Handle(GetMonthlyProgressQuery query, CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not { } userId)
            return Result.Failure<MonthlyProgressResponse>(
                Error.Unauthorized("Workout.Unauthorized", "You must be logged in to view progress."));

        var monthStart = new DateTime(query.Year, query.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

        var workouts = await workoutRepository.GetByUserAndDateRangeAsync(userId, monthStart, monthEnd, cancellationToken);
        var workoutsByWeekStart = workouts.ToLookup(w => GetWeekStart(DateOnly.FromDateTime(w.PerformedAt)));

        var monthStartDate = DateOnly.FromDateTime(monthStart);
        var monthEndDate = DateOnly.FromDateTime(monthEnd);

        var weeks = new List<WeeklyProgressResponse>();
        for (var weekStart = GetWeekStart(monthStartDate); weekStart <= monthEndDate; weekStart = weekStart.AddDays(7))
        {
            var weekEnd = weekStart.AddDays(6);
            var weekWorkouts = workoutsByWeekStart[weekStart].ToList();

            weeks.Add(new WeeklyProgressResponse(
                WeekStart: weekStart < monthStartDate ? monthStartDate : weekStart,
                WeekEnd: weekEnd > monthEndDate ? monthEndDate : weekEnd,
                WorkoutCount: weekWorkouts.Count,
                TotalDurationMinutes: weekWorkouts.Sum(w => w.DurationMinutes),
                AverageDifficulty: weekWorkouts.Count > 0 ? Math.Round(weekWorkouts.Average(w => w.Difficulty.Value), 2) : 0,
                AverageFatigue: weekWorkouts.Count > 0 ? Math.Round(weekWorkouts.Average(w => w.Fatigue.Value), 2) : 0));
        }

        return Result.Success(new MonthlyProgressResponse(query.Year, query.Month, weeks));
    }

    private static DateOnly GetWeekStart(DateOnly date)
    {
        var offsetFromMonday = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-offsetFromMonday);
    }
}