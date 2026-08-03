using WorkoutApp.Domain.Entities;

namespace WorkoutApp.Domain.Tests.Entities;

public class UserTests
{
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidPasswordHash = "hashed-password";

    [Fact]
    public void Register_WithValidData_ReturnsSuccessWithExpectedProperties()
    {
        var result = User.Register(ValidFirstName, ValidLastName, ValidEmail, ValidPasswordHash);

        Assert.True(result.IsSuccess);
        var user = result.Value;
        Assert.Equal(ValidFirstName, user.FirstName.Value);
        Assert.Equal(ValidLastName, user.LastName.Value);
        Assert.Equal(ValidEmail, user.Email.Value);
        Assert.Equal(ValidPasswordHash, user.PasswordHash);
        Assert.Equal($"{ValidFirstName} {ValidLastName}", user.FullName);
        Assert.False(user.IsDeleted);
        Assert.Null(user.DeletedAt);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public void Register_WithInvalidFirstName_ReturnsFailure()
    {
        var result = User.Register("", ValidLastName, ValidEmail, ValidPasswordHash);

        Assert.True(result.IsFailure);
        Assert.Equal("PersonName.Empty", result.Error.Code);
    }

    [Fact]
    public void Register_WithInvalidLastName_ReturnsFailure()
    {
        var result = User.Register(ValidFirstName, "", ValidEmail, ValidPasswordHash);

        Assert.True(result.IsFailure);
        Assert.Equal("PersonName.Empty", result.Error.Code);
    }

    [Fact]
    public void Register_WithInvalidEmail_ReturnsFailure()
    {
        var result = User.Register(ValidFirstName, ValidLastName, "not-an-email", ValidPasswordHash);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
    }

    [Fact]
    public void Delete_SetsIsDeletedAndDeletedAt()
    {
        var user = User.Register(ValidFirstName, ValidLastName, ValidEmail, ValidPasswordHash).Value;

        user.Delete();

        Assert.True(user.IsDeleted);
        Assert.NotNull(user.DeletedAt);
    }

    [Fact]
    public void UpdateProfile_WithValidData_UpdatesFields()
    {
        var user = User.Register(ValidFirstName, ValidLastName, ValidEmail, ValidPasswordHash).Value;

        var result = user.UpdateProfile("Jane", "Smith", "jane.smith@example.com");

        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", user.FirstName.Value);
        Assert.Equal("Smith", user.LastName.Value);
        Assert.Equal("jane.smith@example.com", user.Email.Value);
    }

    [Fact]
    public void UpdateProfile_WithInvalidEmail_ReturnsFailureAndLeavesFieldsUnchanged()
    {
        var user = User.Register(ValidFirstName, ValidLastName, ValidEmail, ValidPasswordHash).Value;

        var result = user.UpdateProfile("Jane", "Smith", "not-an-email");

        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
        Assert.Equal(ValidFirstName, user.FirstName.Value);
        Assert.Equal(ValidLastName, user.LastName.Value);
        Assert.Equal(ValidEmail, user.Email.Value);
    }

    [Fact]
    public void UpdateProfile_WithInvalidFirstName_ReturnsFailureAndLeavesFieldsUnchanged()
    {
        var user = User.Register(ValidFirstName, ValidLastName, ValidEmail, ValidPasswordHash).Value;

        var result = user.UpdateProfile("", "Smith", "jane.smith@example.com");

        Assert.True(result.IsFailure);
        Assert.Equal("PersonName.Empty", result.Error.Code);
        Assert.Equal(ValidFirstName, user.FirstName.Value);
    }
}