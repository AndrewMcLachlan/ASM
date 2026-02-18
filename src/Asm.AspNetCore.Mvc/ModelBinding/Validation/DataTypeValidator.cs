using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Asm.AspNetCore.Mvc.ModelBinding.Validation;

/// <summary>
/// Validates against data type.
/// </summary>
public class DataTypeValidatorAttribute : DataTypeAttribute, IClientModelValidator
{
    #region Constants
    private const string EmailValidationRegExString = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
    private static readonly Regex EmailRegEx = new Regex(EmailValidationRegExString);
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeValidatorAttribute"/> class.
    /// </summary>
    /// <param name="customDataType">A custom data type.</param>
    public DataTypeValidatorAttribute(string customDataType) : base(customDataType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeValidatorAttribute"/> class.
    /// </summary>
    /// <param name="dataType">The expected data type.</param>
    public DataTypeValidatorAttribute(DataType dataType) : base(dataType)
    {
    }

    /// <summary>
    /// Adda client side validation rule.
    /// </summary>
    /// <param name="context"></param>
    public void AddValidation(ClientModelValidationContext context)
    {
        if (DataType == DataType.EmailAddress)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-regex", FormatErrorMessage(context.ModelMetadata.DisplayName ?? String.Empty));
            context.Attributes.Add("data-val-regex-pattern", EmailValidationRegExString);
        }
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
        if (DataType == DataType.EmailAddress && value is string str && !String.IsNullOrEmpty(str) && !EmailRegEx.Match(str).Success)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        return base.IsValid(value, validationContext);
    }
    #endregion
}
