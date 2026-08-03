using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Enums;

namespace WorkoutApp.Domain.Tests.Common;

public class ValidationErrorTests
{
    [Fact]
    public void FromErrors_SetsErrorsAndFixedCodeMessageType()
    {
        var errors = new List<Error>
        {
            Error.Validation("Field.Required", "Field is required."),
            Error.Validation("Field.TooLong", "Field is too long.")
        };

        var validationError = ValidationError.FromErrors(errors);

        Assert.Equal("Validation.General", validationError.Code);
        Assert.Equal("One or more validation errors occurred.", validationError.Message);
        Assert.Equal(ErrorType.Validation, validationError.Type);
        Assert.Equal(errors, validationError.Errors);
    }

    [Fact]
    public void FromErrors_WithEmptyList_HasNoErrors()
    {
        var validationError = ValidationError.FromErrors([]);

        Assert.Empty(validationError.Errors);
    }
}