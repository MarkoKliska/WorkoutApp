using Microsoft.EntityFrameworkCore;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Infrastructure.Persistence.Repositories;

namespace WorkoutApp.Infrastructure.Tests.Persistence.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsSuccess()
    {
        await using var context = TestDbContextFactory.Create();
        var user = User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context);

        var result = await repository.GetByIdAsync(user.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new UserRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("User.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserIsSoftDeleted_ReturnsNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var user = User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;
        user.Delete();
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context);

        var result = await repository.GetByIdAsync(user.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("User.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetByEmailAsync_IsCaseInsensitive()
    {
        await using var context = TestDbContextFactory.Create();
        var user = User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync("JOHN.DOE@EXAMPLE.COM");

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmailFormat_ReturnsValidationFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync("not-an-email");

        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
    }

    [Fact]
    public async Task Add_PersistsUserAfterSaveChanges()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new UserRepository(context);
        var user = User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;

        repository.Add(user);
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Users.CountAsync());
    }
}