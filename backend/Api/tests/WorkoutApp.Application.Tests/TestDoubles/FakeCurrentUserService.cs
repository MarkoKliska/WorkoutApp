using WorkoutApp.Application.Interfaces;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakeCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
}