#pragma warning disable CA1822 // Mark members as static
using System.Net;

namespace Asm.Reqnroll;

/// <summary>
/// Step argument transformations for Reqnroll.
/// </summary>
[Binding]
public class StepArgumentTransformations
{
    /// <summary>
    /// Represents a null string.
    /// </summary>
    public const string NullString = "<NULL>";

    /// <summary>
    /// Converts a string to <c>null</c>.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    [StepArgumentTransformation(NullString)]

    public string? ToStringNull() => null;

    /// <summary>
    /// Converts a string to a <c>null</c> integer.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    [StepArgumentTransformation(NullString)]
    public int? ToIntNull() => null;

    /// <summary>
    /// Converts a string to a <c>null</c> long.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    [StepArgumentTransformation(NullString)]
    public long? ToLongNull() => null;

    /// <summary>
    /// Converts a string to a <c>null</c> decimal.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    [StepArgumentTransformation(NullString)]
    public decimal? ToDecimalNull() => null;

    /// <summary>
    /// Converts a string to a <c>null</c> double.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    [StepArgumentTransformation(NullString)]
    public double? ToDoubleNull() => null;

    /// <summary>
    /// Converts a string to <see cref="IPAddress"/>.
    /// </summary>
    /// <param name="input">The string to convert</param>
    /// <returns></returns>
    [StepArgumentTransformation(@"^(\d+\.\d+\.\d+\.\d+)$")]
    [StepArgumentTransformation(@"^((?:[0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|(?:[0-9a-fA-F]{1,4}:){1,7}:|(?:[0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|(?:[0-9a-fA-F]{1,4}:){1,5}(?::[0-9a-fA-F]{1,4}){1,2}|(?:[0-9a-fA-F]{1,4}:){1,4}(?::[0-9a-fA-F]{1,4}){1,3}|(?:[0-9a-fA-F]{1,4}:){1,3}(?::[0-9a-fA-F]{1,4}){1,4}|(?:[0-9a-fA-F]{1,4}:){1,2}(?::[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:(?:(?::[0-9a-fA-F]{1,4}){1,6})|:(?:(?::[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(?::[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(?:ffff(?::0{1,4}){0,1}:){0,1}(?:(?:25[0-5]|(?:2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(?:25[0-5]|(?:2[0-4]|1{0,1}[0-9]){0,1}[0-9])|(?:[0-9a-fA-F]{1,4}:){1,4}:(?:(?:25[0-5]|(?:2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(?:25[0-5]|(?:2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$")]
    public IPAddress ToIPAddress(string input) => IPAddress.Parse(input);
}
#pragma warning restore CA1822 // Mark members as static
