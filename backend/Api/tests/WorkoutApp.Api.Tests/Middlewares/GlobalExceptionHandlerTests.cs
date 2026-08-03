using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WorkoutApp.Api.Middlewares;
using WorkoutApp.Api.Tests.TestDoubles;

namespace WorkoutApp.Api.Tests.Middlewares;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_SetsResponseStatusCodeTo500()
    {
        var handler = new GlobalExceptionHandler(new FakeProblemDetailsService(), new FakeLogger<GlobalExceptionHandler>());
        var httpContext = new DefaultHttpContext();

        await handler.TryHandleAsync(httpContext, new InvalidOperationException("Something broke."), CancellationToken.None);

        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_LogsExceptionAsError()
    {
        var logger = new FakeLogger<GlobalExceptionHandler>();
        var handler = new GlobalExceptionHandler(new FakeProblemDetailsService(), logger);
        var exception = new InvalidOperationException("Something broke.");

        await handler.TryHandleAsync(new DefaultHttpContext(), exception, CancellationToken.None);

        var logEntry = Assert.Single(logger.LogEntries);
        Assert.Equal(LogLevel.Error, logEntry.Level);
        Assert.Same(exception, logEntry.Exception);
    }

    [Fact]
    public async Task TryHandleAsync_WritesGenericProblemDetailsWithoutLeakingExceptionMessage()
    {
        var problemDetailsService = new FakeProblemDetailsService();
        var handler = new GlobalExceptionHandler(problemDetailsService, new FakeLogger<GlobalExceptionHandler>());

        await handler.TryHandleAsync(new DefaultHttpContext(), new Exception("Sensitive internal detail"), CancellationToken.None);

        var problemDetails = problemDetailsService.CapturedContext?.ProblemDetails;
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("An unexpected error occurred.", problemDetails.Title);
        Assert.DoesNotContain("Sensitive internal detail", problemDetails.Detail ?? string.Empty);
    }

    [Fact]
    public async Task TryHandleAsync_ReturnsWhateverProblemDetailsServiceReturns()
    {
        var problemDetailsService = new FakeProblemDetailsService { ReturnValue = false };
        var handler = new GlobalExceptionHandler(problemDetailsService, new FakeLogger<GlobalExceptionHandler>());

        var handled = await handler.TryHandleAsync(new DefaultHttpContext(), new Exception(), CancellationToken.None);

        Assert.False(handled);
    }
}