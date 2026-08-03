using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WorkoutApp.Infrastructure.Services;

namespace WorkoutApp.Infrastructure.Tests.Services;

public class CurrentUserServiceTests
{
    private static CurrentUserService CreateService(HttpContext? httpContext)
    {
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        return new CurrentUserService(accessor);
    }

    [Fact]
    public void UserId_WithNoHttpContext_ReturnsNull()
    {
        var service = CreateService(null);

        Assert.Null(service.UserId);
    }

    [Fact]
    public void UserId_WithNoAuthenticatedUser_ReturnsNull()
    {
        var httpContext = new DefaultHttpContext();
        var service = CreateService(httpContext);

        Assert.Null(service.UserId);
    }

    [Fact]
    public void UserId_WithValidNameIdentifierClaim_ReturnsParsedGuid()
    {
        var userId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, userId.ToString())]))
        };
        var service = CreateService(httpContext);

        Assert.Equal(userId, service.UserId);
    }

    [Fact]
    public void UserId_WithNonGuidNameIdentifierClaim_ReturnsNull()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "not-a-guid")]))
        };
        var service = CreateService(httpContext);

        Assert.Null(service.UserId);
    }
}