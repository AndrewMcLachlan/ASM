using System.ComponentModel.DataAnnotations;
using Asm.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Mvc.Tests.ModelBinding;

[Trait("Category", "Unit")]

public class DataTypeValidatorTests
{
    private static ValidationResult? Validate(object? value, DataType dataType = DataType.EmailAddress)
    {
        var attribute = new DataTypeValidatorAttribute(dataType);
        var context = new ValidationContext(new object()) { DisplayName = "Email" };
        return attribute.GetValidationResult(value, context);
    }

    private static ClientModelValidationContext ClientContext(out IDictionary<string, string> attributes, ModelMetadata? metadata = null)
    {
        var provider = new EmptyModelMetadataProvider();
        var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
        attributes = new Dictionary<string, string>();
        return new ClientModelValidationContext(actionContext, metadata ?? provider.GetMetadataForType(typeof(string)), provider, attributes);
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

    /// <summary>
    /// Given an email-address value that is empty
    /// When it is validated
    /// Then the empty-string short-circuit is taken and validation succeeds
    /// </summary>
    [Fact]
    public void EmptyValuePassesValidation()
    {
        Assert.Equal(ValidationResult.Success, Validate(""));
    }

    /// <summary>
    /// Given a value that is not a string
    /// When it is validated as an email address
    /// Then the string type-check short-circuits to the base validator and validation succeeds
    /// </summary>
    [Fact]
    public void NonStringValuePassesValidation()
    {
        Assert.Equal(ValidationResult.Success, Validate(42));
    }

    /// <summary>
    /// Given a validator for a data type other than EmailAddress
    /// When a malformed-email string is validated
    /// Then the email rule is not applied and validation succeeds
    /// </summary>
    [Fact]
    public void NonEmailDataTypeSkipsEmailRule()
    {
        Assert.Equal(ValidationResult.Success, Validate("not-an-email", DataType.Text));
    }

    /// <summary>
    /// Given an EmailAddress validator
    /// When client validation metadata is added
    /// Then the regex data-val attributes are emitted
    /// </summary>
    [Fact]
    public void AddValidationEmitsEmailRegexAttributes()
    {
        var attribute = new DataTypeValidatorAttribute(DataType.EmailAddress);
        var context = ClientContext(out var attributes);

        attribute.AddValidation(context);

        Assert.Equal("true", attributes["data-val"]);
        Assert.True(attributes.ContainsKey("data-val-regex"));
        Assert.True(attributes.ContainsKey("data-val-regex-pattern"));
    }

    /// <summary>
    /// Given a validator for a data type other than EmailAddress
    /// When client validation metadata is added
    /// Then no attributes are emitted
    /// </summary>
    [Fact]
    public void AddValidationEmitsNothingForNonEmailDataType()
    {
        var attribute = new DataTypeValidatorAttribute(DataType.Text);
        var context = ClientContext(out var attributes);

        attribute.AddValidation(context);

        Assert.Empty(attributes);
    }

    /// <summary>
    /// Given model metadata that carries a display name
    /// When client validation metadata is added for an email address
    /// Then the display name flows into the formatted error message
    /// </summary>
    [Fact]
    public void AddValidationUsesModelDisplayName()
    {
        // A real MVC metadata provider (unlike EmptyModelMetadataProvider) reads [Display] annotations.
        using var services = new ServiceCollection().AddMvcCore().AddDataAnnotations().Services.BuildServiceProvider();
        var provider = services.GetRequiredService<IModelMetadataProvider>();
        var metadata = provider.GetMetadataForProperty(typeof(Model), nameof(Model.Email));
        var attribute = new DataTypeValidatorAttribute(DataType.EmailAddress);
        var context = ClientContext(out var attributes, metadata);

        attribute.AddValidation(context);

        Assert.Contains("Email Address", attributes["data-val-regex"]);
    }

    private sealed class Model
    {
        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";
    }
}
