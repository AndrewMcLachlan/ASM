using System.ComponentModel.DataAnnotations;

namespace Asm.AspNetCore.Mvc.ModelBinding.Validation;

/// <summary>
/// Validates against data type.
/// </summary>
public class BannedWordsValidatorAttribute : ValidationAttribute
{
    /// <summary>
    /// Gets or sets the banned words.
    /// </summary>
    public IEnumerable<string> BannedWords { get; } = [];

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="BannedWordsValidatorAttribute"/> class.
    /// </summary>
    /// <param name="bannedWords">The banned words.</param>
    public BannedWordsValidatorAttribute(params string[] bannedWords) : base()
    {
        BannedWords = bannedWords;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BannedWordsValidatorAttribute"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public BannedWordsValidatorAttribute(string errorMessage) : base(errorMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BannedWordsValidatorAttribute"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="bannedWords">The banned words.</param>
    public BannedWordsValidatorAttribute(string errorMessage, params string[] bannedWords) : base(errorMessage)
    {
        BannedWords = bannedWords;
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Tests to see if this rule is valid.
    /// </summary>
    /// <param name="value">The vale to test.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A validation result.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string text && BannedWords.Any(b => text.Contains(b, StringComparison.InvariantCultureIgnoreCase)))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        return ValidationResult.Success;
    }
    #endregion
}
