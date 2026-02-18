using FluentValidation;

namespace Asm.AspNetCore.Validators;

/// <summary>
/// A validator for objects that implement <see cref="IDescribed"/> and <see cref="INamed"/>.
/// </summary>
/// <typeparam name="T">The type of object to be validated.</typeparam>
public class DescribedValidator<T> : AbstractValidator<T> where T : IDescribed, INamed
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DescribedValidator{T}"/> class.
    /// </summary>
    public DescribedValidator() : this(50, 255) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DescribedValidator{T}"/> class.
    /// </summary>
    /// <param name="nameLength">The maximum allowed length of the name.</param>
    /// <param name="descriptionLength">The maximum allowed length of the description.</param>
    public DescribedValidator(int nameLength, int descriptionLength)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(nameLength);
        RuleFor(x => x.Description).MaximumLength(descriptionLength);
    }
}
