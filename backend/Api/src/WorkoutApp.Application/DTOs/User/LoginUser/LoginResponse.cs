namespace WorkoutApp.Application.DTOs.User.LoginUser;

public sealed record LoginResponse(Guid UserId, string Token);