using FluentValidation;

namespace WorkoutApp.Application.Features.Workouts.Commands.LogWorkout;

public sealed class LogWorkoutCommandValidator : AbstractValidator<LogWorkoutCommand>
{
    public LogWorkoutCommandValidator()
    {
        RuleFor(x => x.Request.ExerciseType).IsInEnum();
        RuleFor(x => x.Request.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.Request.CaloriesBurned).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Request.Difficulty).InclusiveBetween(1, 10);
        RuleFor(x => x.Request.Fatigue).InclusiveBetween(1, 10);
        RuleFor(x => x.Request.Notes).MaximumLength(500);
        RuleFor(x => x.Request.PerformedAt).LessThanOrEqualTo(DateTime.UtcNow);
    }
}