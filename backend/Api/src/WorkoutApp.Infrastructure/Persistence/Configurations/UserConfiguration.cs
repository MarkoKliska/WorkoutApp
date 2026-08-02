using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.ValueObjects;

namespace WorkoutApp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasConversion(name => name.Value, value => PersonName.Create(value).Value)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasConversion(name => name.Value, value => PersonName.Create(value).Value)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique().HasFilter("is_deleted = false");

        builder.Property(u => u.PasswordHash).IsRequired();
    }
}