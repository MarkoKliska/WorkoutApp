using FluentValidation;
using MediatR;
using WorkoutApp.Application.Common.Behaviors;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Tests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private static RequestHandlerDelegate<Result> NextReturning(Result result) =>
        _ => Task.FromResult(result);

    private static RequestHandlerDelegate<Result<string>> NextReturning(Result<string> result) =>
        _ => Task.FromResult(result);

    [Fact]
    public async Task Handle_WithNoValidators_CallsNextAndReturnsItsResult()
    {
        var behavior = new ValidationBehavior<TestRequest, Result>([]);
        var expected = Result.Success();

        var result = await behavior.Handle(new TestRequest("anything"), NextReturning(expected), CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task Handle_WithPassingValidators_CallsNextAndReturnsItsResult()
    {
        var behavior = new ValidationBehavior<TestRequest, Result>([new ConfigurableValidator(isValid: true)]);
        var expected = Result.Success();

        var result = await behavior.Handle(new TestRequest("anything"), NextReturning(expected), CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task Handle_WithFailingValidator_ReturnsFailureWithoutCallingNext_ForNonGenericResult()
    {
        var behavior = new ValidationBehavior<TestRequest, Result>([new ConfigurableValidator(false, "Value is invalid.")]);
        var nextCalled = false;
        RequestHandlerDelegate<Result> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        };

        var result = await behavior.Handle(new TestRequest("anything"), next, CancellationToken.None);

        Assert.False(nextCalled);
        Assert.True(result.IsFailure);
        var validationError = Assert.IsType<ValidationError>(result.Error);
        Assert.Single(validationError.Errors);
        Assert.Equal("Value is invalid.", validationError.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WithFailingValidator_ReturnsFailureWithoutCallingNext_ForGenericResult()
    {
        var behavior = new ValidationBehavior<TestRequest, Result<string>>([new ConfigurableValidator(false, "Value is invalid.")]);
        var nextCalled = false;
        RequestHandlerDelegate<Result<string>> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(Result.Success("unused"));
        };

        var result = await behavior.Handle(new TestRequest("anything"), next, CancellationToken.None);

        Assert.False(nextCalled);
        Assert.True(result.IsFailure);
        Assert.Throws<InvalidOperationException>(() => result.Value);
        var validationError = Assert.IsType<ValidationError>(result.Error);
        Assert.Single(validationError.Errors);
    }

    [Fact]
    public async Task Handle_WithMultipleFailingValidators_AggregatesAllErrors()
    {
        var behavior = new ValidationBehavior<TestRequest, Result>([
            new ConfigurableValidator(false, "First error."),
            new ConfigurableValidator(false, "Second error.")
        ]);

        var result = await behavior.Handle(new TestRequest("anything"), NextReturning(Result.Success()), CancellationToken.None);

        Assert.True(result.IsFailure);
        var validationError = Assert.IsType<ValidationError>(result.Error);
        Assert.Equal(2, validationError.Errors.Count);
        Assert.Contains(validationError.Errors, e => e.Message == "First error.");
        Assert.Contains(validationError.Errors, e => e.Message == "Second error.");
    }
}

file sealed record TestRequest(string Value);

file sealed class ConfigurableValidator : AbstractValidator<TestRequest>
{
    public ConfigurableValidator(bool isValid, string errorMessage = "Value is invalid.")
    {
        RuleFor(x => x.Value).Must(_ => isValid).WithMessage(errorMessage);
    }
}