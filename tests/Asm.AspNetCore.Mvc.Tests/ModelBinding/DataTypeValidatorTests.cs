using System.ComponentModel.DataAnnotations;
using Asm.AspNetCore.Mvc.ModelBinding.Validation;

namespace Asm.AspNetCore.Mvc.Tests.ModelBinding;

public class DataTypeValidatorTests
{
    private static ValidationResult? Validate(string value)
    {
        var attribute = new DataTypeValidatorAttribute(DataType.EmailAddress);
        var context = new ValidationContext(new object()) { DisplayName = "Email" };
        return attribute.GetValidationResult(value, context);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("USER@EXAMPLE.COM")]        // uppercase must be accepted
    [InlineData("John.Doe@Example.Co.Uk")]  // mixed case
    public void ValidEmail_PassesValidation(string email)
    {
        Assert.Equal(ValidationResult.Success, Validate(email));
    }

    [Theory]
    [InlineData("<script>x</script> a@b.co")] // embedded valid substring must be rejected
    [InlineData("a@b.co and more")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void InvalidEmail_FailsValidation(string email)
    {
        Assert.NotEqual(ValidationResult.Success, Validate(email));
    }
}
