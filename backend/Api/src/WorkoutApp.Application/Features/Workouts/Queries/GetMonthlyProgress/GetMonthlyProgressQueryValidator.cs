using FluentValidation;

namespace WorkoutApp.Application.Features.Workouts.Queries.GetMonthlyProgress;

public sealed class GetMonthlyProgressQueryValidator : AbstractValidator<GetMonthlyProgressQuery>
{
    public GetMonthlyProgressQueryValidator()
    {
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}