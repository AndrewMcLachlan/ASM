using System.ComponentModel.DataAnnotations;
using Asm.AspNetCore.Mvc.ModelBinding.Validation;

namespace Asm.AspNetCore.Mvc.Tests.ModelBinding;

[Trait("Category", "Unit")]
public class BannedWordsValidatorTests
{
    private static ValidationResult Validate(object value, params string[] bannedWords)
    {
        var attribute = new BannedWordsValidatorAttribute(bannedWords);
        var context = new ValidationContext(new object()) { DisplayName = "Field" };
        return attribute.GetValidationResult(value, context);
    }

    /// <summary>
    /// Given a value that contains a banned word in any casing
    /// When the value is validated
    /// Then validation does not succeed
    /// </summary>
    [Theory]
    [InlineData("this is spam")]
    [InlineData("THIS IS SPAM")]
    [InlineData("Contains Spam here")]
    public void ValueContainingBannedWordFailsValidation(string value)
    {
        Assert.NotEqual(ValidationResult.Success, Validate(value, "spam"));
    }

    /// <summary>
    /// Given a value that contains none of the banned words
    /// When the value is validated
    /// Then validation succeeds
    /// </summary>
    [Fact]
    public void ValueWithoutBannedWordPassesValidation()
    {
        Assert.Equal(ValidationResult.Success, Validate("perfectly fine", "spam", "junk"));
    }

    /// <summary>
    /// Given a non-string value
    /// When the value is validated against banned words
    /// Then validation succeeds because matching only applies to strings
    /// </summary>
    [Fact]
    public void NonStringValuePassesValidation()
    {
        Assert.Equal(ValidationResult.Success, Validate(42, "spam"));
    }

    /// <summary>
    /// Given a validator configured with no banned words
    /// When any string value is validated
    /// Then validation succeeds
    /// </summary>
    [Fact]
    public void NoBannedWordsConfiguredPassesValidation()
    {
        Assert.Equal(ValidationResult.Success, Validate("anything"));
    }
}
