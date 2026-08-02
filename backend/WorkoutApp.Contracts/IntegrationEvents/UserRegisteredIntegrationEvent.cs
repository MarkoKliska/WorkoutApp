namespace WorkoutApp.Contracts.IntegrationEvents;

public sealed record UserRegisteredIntegrationEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTimeOffset RegisteredAtUtc);