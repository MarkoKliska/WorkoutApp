namespace WorkoutApp.Application.DTOs.User.GetProfile;

public sealed record ProfileResponse(Guid Id, string FirstName, string LastName, string Email);