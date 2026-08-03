using MediatR;

namespace WorkoutApp.Application.Features.Users.Events;

public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName) : INotification;