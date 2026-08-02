using FluentValidation;
using MassTransit;
using WorkoutApp.Contracts.IntegrationEvents;
using WorkoutApp.Mail.Service.Services;

namespace WorkoutApp.Mail.Service.Consumers;

public sealed class UserRegisteredConsumer(
    IValidator<UserRegisteredIntegrationEvent> validator,
    IEmailSender emailSender,
    ILogger<UserRegisteredConsumer> logger)
    : IConsumer<UserRegisteredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        var message = context.Message;

        await validator.ValidateAndThrowAsync(message, context.CancellationToken);

        const string subject = "Welcome to WorkoutApp!";
        var htmlBody = $"""
            <h1>Welcome, {message.FirstName}!</h1>
            <p>Your WorkoutApp account has been created. Time to log your first workout.</p>
            """;

        await emailSender.SendAsync(
            message.Email, $"{message.FirstName} {message.LastName}", subject, htmlBody, context.CancellationToken);

        logger.LogInformation("Welcome email sent to {Email} for user {UserId}", message.Email, message.UserId);
    }
}