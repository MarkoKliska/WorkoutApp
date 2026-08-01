using FluentValidation;

namespace WorkoutApp.Application.Features.Users.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Request.Password).NotEmpty().MinimumLength(8);
    }
}