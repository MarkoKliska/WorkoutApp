using Microsoft.EntityFrameworkCore;
using WorkoutApp.Infrastructure.Persistence.Contexts;

namespace WorkoutApp.Infrastructure.Tests.Persistence;

public static class TestDbContextFactory
{
    public static WorkoutAppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<WorkoutAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WorkoutAppDbContext(options);
    }
}