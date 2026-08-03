using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Api.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess ? new NoContentResult() : CreateErrorResponse(result.Error);

    public static IActionResult ToActionResult<TValue>(this Result<TValue> result) =>
        result.IsSuccess ? new OkObjectResult(result.Value) : CreateErrorResponse(result.Error);

    private static IActionResult CreateErrorResponse(Error error)
    {
        if (error is ValidationError validationError)
        {
            var modelState = new ModelStateDictionary();
            foreach (var failure in validationError.Errors)
                modelState.AddModelError(failure.Code, failure.Message);

            return new BadRequestObjectResult(new ValidationProblemDetails(modelState));
        }

        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return new ObjectResult(new ProblemDetails { Status = statusCode, Title = error.Code, Detail = error.Message })
        {
            StatusCode = statusCode
        };
    }
}