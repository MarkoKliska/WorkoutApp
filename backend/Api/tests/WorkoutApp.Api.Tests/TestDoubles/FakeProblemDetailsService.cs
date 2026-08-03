using Microsoft.AspNetCore.Http;

namespace WorkoutApp.Api.Tests.TestDoubles;

public sealed class FakeProblemDetailsService : IProblemDetailsService
{
    public bool ReturnValue { get; set; } = true;
    public ProblemDetailsContext? CapturedContext { get; private set; }

    public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
    {
        CapturedContext = context;
        return ValueTask.FromResult(ReturnValue);
    }

    public ValueTask WriteAsync(ProblemDetailsContext context)
    {
        CapturedContext = context;
        return ValueTask.CompletedTask;
    }
}