using FluentValidation;

namespace WorkoutApp.Application.Features.Users.Commands.DeleteAccount;

public sealed class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.Request.Password).NotEmpty();
    }
}