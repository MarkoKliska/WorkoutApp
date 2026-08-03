using WorkoutApp.Application.Interfaces;

namespace WorkoutApp.Application.Tests.TestDoubles;

public sealed class FakePasswordHasher : IPasswordHasher
{
    private const string Prefix = "hashed:";

    public string Hash(string password) => Prefix + password;

    public bool Verify(string password, string passwordHash) => passwordHash == Prefix + password;
}