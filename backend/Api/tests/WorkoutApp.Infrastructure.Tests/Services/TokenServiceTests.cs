using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Infrastructure.Options;
using WorkoutApp.Infrastructure.Services;

namespace WorkoutApp.Infrastructure.Tests.Services;

public class TokenServiceTests
{
    private readonly JwtOptions _jwtOptions = new()
    {
        SecretKey = "this-is-a-sufficiently-long-test-secret-key-123456",
        Issuer = "WorkoutApp.Tests",
        Audience = "WorkoutApp.Tests.Client",
        ExpiryMinutes = 60
    };

    private TokenService CreateTokenService() => new(Microsoft.Extensions.Options.Options.Create(_jwtOptions));

    private static User CreateUser() =>
        User.Register("John", "Doe", "john.doe@example.com", "hashed").Value;

    [Fact]
    public void GenerateToken_ReturnsNonEmptyString()
    {
        var token = CreateTokenService().GenerateToken(CreateUser());

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateToken_IncludesExpectedClaimsIssuerAndAudience()
    {
        var user = CreateUser();
        var token = CreateTokenService().GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(_jwtOptions.Issuer, jwt.Issuer);
        Assert.Contains(_jwtOptions.Audience, jwt.Audiences);
        Assert.Equal(user.Id.ToString(), jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Email.Value, jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Email).Value);
    }

    [Fact]
    public void GenerateToken_SetsExpiryAccordingToConfiguredMinutes()
    {
        var token = CreateTokenService().GenerateToken(CreateUser());
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var expectedExpiry = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
        Assert.True(Math.Abs((jwt.ValidTo - expectedExpiry).TotalSeconds) < 5);
    }
}