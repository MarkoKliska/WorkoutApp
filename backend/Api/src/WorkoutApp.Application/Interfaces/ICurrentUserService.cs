namespace WorkoutApp.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}