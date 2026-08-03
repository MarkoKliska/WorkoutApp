using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakeTokenService : ITokenService
{
    public string GenerateToken(User user) => $"token-for-{user.Id}";
}