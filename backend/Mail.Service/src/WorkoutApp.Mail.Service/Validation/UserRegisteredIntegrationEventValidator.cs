using FluentValidation;
using WorkoutApp.Contracts.IntegrationEvents;

namespace WorkoutApp.Mail.Service.Validation;

public sealed class UserRegisteredIntegrationEventValidator : AbstractValidator<UserRegisteredIntegrationEvent>
{
    public UserRegisteredIntegrationEventValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
    }
}