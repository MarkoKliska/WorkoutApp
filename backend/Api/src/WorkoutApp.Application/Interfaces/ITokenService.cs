using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}