using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Extensions;

/// <summary>
/// Extension helpers for <see cref="IPublishedContent"/>.
/// </summary>
public static class IPublishedContentExtensions
{
    /// <summary>
    /// Returns the content item's <c>Name</c> as a lowercase, hyphen-separated CSS class name.
    /// </summary>
    /// <param name="model">The content item.</param>
    /// <returns>The CSS-safe class name, or <c>null</c> if the model is null.</returns>
    public static string? NameAsCssClass(this IPublishedContent model) =>
        model?.Name.ToLower().Replace(' ', '-');

    /// <summary>
    /// Returns the value of the named property if it has a value, otherwise the supplied default.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="model">The content item.</param>
    /// <param name="alias">The property alias.</param>
    /// <param name="other">The fallback value if the property has no value.</param>
    /// <returns>The property value or the fallback.</returns>
    public static T? ValueOr<T>(this IPublishedContent model, string alias, T other) =>
        model.HasValue(alias) ? model.Value<T>(alias) : other;

    /// <summary>
    /// Returns the supplied format string with the named property's value substituted in,
    /// or an empty string if the property has no value.
    /// </summary>
    /// <param name="model">The content item.</param>
    /// <param name="alias">The property alias.</param>
    /// <param name="format">A composite format string with a single placeholder.</param>
    /// <returns>The formatted string, or <see cref="String.Empty"/>.</returns>
    public static string ValueAnd(this IPublishedContent model, string alias, string format) =>
        model.HasValue(alias) ? String.Format(format, model.Value<string>(alias)) : String.Empty;
}
