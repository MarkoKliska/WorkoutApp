using MassTransit;
using MediatR;
using WorkoutApp.Application.Features.Users.Events;
using WorkoutApp.Contracts.IntegrationEvents;

namespace WorkoutApp.Infrastructure.Messaging;

public sealed class UserRegisteredEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<UserRegisteredEvent>
{
    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        return publishEndpoint.Publish(
            new UserRegisteredIntegrationEvent(
                notification.UserId,
                notification.Email,
                notification.FirstName,
                notification.LastName,
                DateTimeOffset.UtcNow),
            cancellationToken);
    }
}