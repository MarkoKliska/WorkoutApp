using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Infrastructure.Persistence.Contexts;

public class WorkoutAppDbContext(DbContextOptions<WorkoutAppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Workout> Workouts => Set<Workout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkoutAppDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var condition = Expression.Lambda(Expression.Not(property), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(condition);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}