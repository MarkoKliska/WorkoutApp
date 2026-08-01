using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        List<ValidationFailure> failures = [];
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
            return await next();

        var errors = failures
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToList();

        return CreateValidationFailure<TResponse>(ValidationError.FromErrors(errors));
    }

    private static TResult CreateValidationFailure<TResult>(ValidationError validationError)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
            return (TResult)(object)Result.Failure(validationError);

        var valueType = typeof(TResult).GetGenericArguments()[0];
        var failureMethod = typeof(Result)
            .GetMethods()
            .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethodDefinition)
            .MakeGenericMethod(valueType);

        return (TResult)failureMethod.Invoke(null, [validationError])!;
    }
}