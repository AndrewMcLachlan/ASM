namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extensions for the <see cref="IHeaderDictionary"/> interface.
/// </summary>
public static class IHeaderDictionaryExtensions
{
    /// <summary>
    /// Appends the specified header only when <paramref name="value"/> is not null.
    /// </summary>
    /// <param name="headers">The header dictionary.</param>
    /// <param name="name">The header name.</param>
    /// <param name="value">The header value, or <see langword="null"/> to skip.</param>
    public static void AppendIfNotNull(this IHeaderDictionary headers, string name, string? value)
    {
        if (value is not null)
        {
            headers.Append(name, value);
        }
    }
}
