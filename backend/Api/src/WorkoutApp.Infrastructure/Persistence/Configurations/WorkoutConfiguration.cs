using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.ValueObjects;

namespace WorkoutApp.Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.ExerciseType).HasConversion<string>().IsRequired();

        builder.Property(w => w.Difficulty)
            .HasConversion(rating => rating.Value, value => Rating.Create(value).Value)
            .IsRequired();

        builder.Property(w => w.Fatigue)
            .HasConversion(rating => rating.Value, value => Rating.Create(value).Value)
            .IsRequired();

        builder.Property(w => w.Notes).HasMaxLength(500);

        builder.HasIndex(w => new { w.UserId, w.PerformedAt });
    }
}