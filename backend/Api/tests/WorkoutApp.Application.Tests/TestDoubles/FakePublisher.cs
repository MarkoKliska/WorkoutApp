using MediatR;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakePublisher : IPublisher
{
    private readonly List<object> _publishedNotifications = [];

    public IReadOnlyList<object> PublishedNotifications => _publishedNotifications;

    public Exception? ThrowOnPublish { get; set; }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        if (ThrowOnPublish is { } exception)
            throw exception;

        _publishedNotifications.Add(notification);
        return Task.CompletedTask;
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (ThrowOnPublish is { } exception)
            throw exception;

        _publishedNotifications.Add(notification);
        return Task.CompletedTask;
    }
}