using WorkoutApp.Infrastructure.Services;

namespace WorkoutApp.Infrastructure.Tests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void Hash_ReturnsValueDifferentFromPlainPassword()
    {
        var hash = _passwordHasher.Hash("Password1");

        Assert.NotEqual("Password1", hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void Hash_CalledTwiceWithSamePassword_ProducesDifferentHashes()
    {
        var first = _passwordHasher.Hash("Password1");
        var second = _passwordHasher.Hash("Password1");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Verify_WithCorrectPassword_ReturnsTrue()
    {
        var hash = _passwordHasher.Hash("Password1");

        Assert.True(_passwordHasher.Verify("Password1", hash));
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ReturnsFalse()
    {
        var hash = _passwordHasher.Hash("Password1");

        Assert.False(_passwordHasher.Verify("WrongPassword", hash));
    }
}