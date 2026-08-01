namespace WorkoutApp.Application.DTOs.User.RegisterUser;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);