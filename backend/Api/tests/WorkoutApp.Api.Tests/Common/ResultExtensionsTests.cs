using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Api.Common;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Api.Tests.Common;

public class ResultExtensionsTests
{
    [Fact]
    public void ToActionResult_WithSuccess_ReturnsNoContent()
    {
        var result = Result.Success();

        var actionResult = result.ToActionResult();

        var noContent = Assert.IsType<NoContentResult>(actionResult);
        Assert.Equal(StatusCodes.Status204NoContent, noContent.StatusCode);
    }

    [Fact]
    public void ToActionResultOfT_WithSuccess_ReturnsOkObjectResultWithValue()
    {
        var result = Result.Success("payload");

        var actionResult = result.ToActionResult();

        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal("payload", ok.Value);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
    }

    [Theory]
    [InlineData("NotFound", StatusCodes.Status404NotFound)]
    [InlineData("Conflict", StatusCodes.Status409Conflict)]
    [InlineData("Unauthorized", StatusCodes.Status401Unauthorized)]
    [InlineData("Validation", StatusCodes.Status400BadRequest)]
    [InlineData("Failure", StatusCodes.Status500InternalServerError)]
    public void ToActionResult_WithFailure_MapsErrorTypeToExpectedStatusCode(string errorTypeName, int expectedStatusCode)
    {
        var error = CreatePlainError(errorTypeName);
        var result = Result.Failure(error);

        var actionResult = result.ToActionResult();

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(expectedStatusCode, objectResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(error.Code, problemDetails.Title);
        Assert.Equal(error.Message, problemDetails.Detail);
    }

    [Fact]
    public void ToActionResult_WithValidationError_ReturnsBadRequestWithValidationProblemDetails()
    {
        var validationError = ValidationError.FromErrors([
            Error.Validation("FirstName", "First name is required."),
            Error.Validation("Email", "Email format is invalid.")
        ]);
        var result = Result.Failure(validationError);

        var actionResult = result.ToActionResult();

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        Assert.Equal("First name is required.", Assert.Single(problemDetails.Errors["FirstName"]));
        Assert.Equal("Email format is invalid.", Assert.Single(problemDetails.Errors["Email"]));
    }

    private static Error CreatePlainError(string errorTypeName) => errorTypeName switch
    {
        "NotFound" => Error.NotFound("Test.NotFound", "Not found message."),
        "Conflict" => Error.Conflict("Test.Conflict", "Conflict message."),
        "Unauthorized" => Error.Unauthorized("Test.Unauthorized", "Unauthorized message."),
        "Validation" => Error.Validation("Test.Validation", "Validation message."),
        "Failure" => Error.Failure("Test.Failure", "Failure message."),
        _ => throw new ArgumentOutOfRangeException(nameof(errorTypeName))
    };
}