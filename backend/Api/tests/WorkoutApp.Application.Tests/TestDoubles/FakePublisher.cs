using MediatR;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakePublisher : IPublisher
{
    private readonly List<object> _publishedNotifications = [];

    public IReadOnlyList<object> PublishedNotifications => _publishedNotifications;

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        _publishedNotifications.Add(notification);
        return Task.CompletedTask;
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        _publishedNotifications.Add(notification);
        return Task.CompletedTask;
    }
}