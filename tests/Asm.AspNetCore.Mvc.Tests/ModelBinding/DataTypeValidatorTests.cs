using System.ComponentModel.DataAnnotations;
using Asm.AspNetCore.Mvc.ModelBinding.Validation;

namespace Asm.AspNetCore.Mvc.Tests.ModelBinding;

[Trait("Category", "Unit")]

public class DataTypeValidatorTests
{
    private static ValidationResult? Validate(string value)
    {
        var attribute = new DataTypeValidatorAttribute(DataType.EmailAddress);
        var context = new ValidationContext(new object()) { DisplayName = "Email" };
        return attribute.GetValidationResult(value, context);
    }

    /// <summary>
    /// Given a DataType.EmailAddress validator and a well-formed email address in any casing
    /// When the value is validated
    /// Then validation succeeds
    /// </summary>
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("USER@EXAMPLE.COM")]        // uppercase must be accepted
    [InlineData("John.Doe@Example.Co.Uk")]  // mixed case
    public void ValidEmailPassesValidation(string email)
    {
        Assert.Equal(ValidationResult.Success, Validate(email));
    }

    /// <summary>
    /// Given a DataType.EmailAddress validator and a malformed email value
    /// When the value is validated
    /// Then validation does not succeed
    /// </summary>
    [Theory]
    [InlineData("<script>x</script> a@b.co")] // embedded valid substring must be rejected
    [InlineData("a@b.co and more")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void InvalidEmailFailsValidation(string email)
    {
        Assert.NotEqual(ValidationResult.Success, Validate(email));
    }
}
