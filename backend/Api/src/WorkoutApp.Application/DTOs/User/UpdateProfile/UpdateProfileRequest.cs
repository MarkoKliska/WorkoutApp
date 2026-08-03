namespace WorkoutApp.Application.DTOs.User.UpdateProfile;

public sealed record UpdateProfileRequest(string FirstName, string LastName, string Email);