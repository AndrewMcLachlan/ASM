using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asm.Drawing;

/// <summary>
/// A structure representing a hexadecimal colour value.
/// </summary>
[JsonConverter(typeof(HexColourJsonConverter))]
[CLSCompliant(false)]
public readonly struct HexColour : IEquatable<HexColour>
{
    private readonly uint _value; // Stack-allocated integer instead of heap-allocated string

    /// <summary>
    /// Initializes a new instance of the <see cref="HexColour"/> structure.
    /// </summary>
    /// <param name="hexValue">The hexadecimal colour value as RGB.</param>
    /// <exception cref="ArgumentException">Thrown when the hexValue is null, whitespace, or not in a valid hex colour format.</exception>
    public HexColour(string hexValue)
    {
        if (String.IsNullOrWhiteSpace(hexValue))
            throw new ArgumentException("Hex colour value cannot be null or whitespace.", nameof(hexValue));

        if (!TryParseValue(hexValue, out _value))
            throw new FormatException($"Invalid hex colour format: {hexValue}");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HexColour"/> structure.
    /// </summary>
    /// <param name="value">The hexadecimal colour value as an unsigned integer.</param>
    /// <exception cref="ArgumentException">Thrown when the value exceeds 0xFFFFFF.</exception>
    public HexColour(uint value)
    {
        if (value > 0xFFFFFF)
            throw new ArgumentOutOfRangeException(nameof(value), "Colour value cannot exceed 0xFFFFFF");

        _value = value;
    }

    /// <summary>
    /// Gets the underlying unsigned integer value of the hex colour.
    /// </summary>
    public uint Value => _value;

    /// <summary>
    /// Gets the hexadecimal string representation of the colour in the format "#RRGGBB".
    /// </summary>
    public string HexString => $"#{_value:X6}";

    /// <summary>
    /// Tries to parse a hexadecimal colour string into a <see cref="HexColour"/> instance.
    /// </summary>
    /// <param name="hexValue">The hexadecimal colour string to parse. Must be in the format "#RRGGBB" or "#RGB".</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="HexColour"/> if the parsing succeeded.</param>
    /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? hexValue, out HexColour result)
    {
        if (String.IsNullOrWhiteSpace(hexValue) || !TryParseValue(hexValue, out var value))
        {
            result = default;
            return false;
        }

        result = new HexColour(value);
        return true;
    }

    /// <summary>
    /// Parses a hexadecimal colour string into a <see cref="HexColour"/> instance.
    /// </summary>
    /// <param name="hexValue">The hexadecimal colour string to parse. Must be in the format "#RRGGBB" or "#RGB".</param>
    /// <exception cref="ArgumentException">Thrown when the hexValue is null, whitespace, or not in a valid hex colour format.</exception>
    /// <returns>The parsed <see cref="HexColour"/> instance.</returns>
    public static HexColour Parse(string hexValue) => new(hexValue);

    /// <summary>
    /// Parses a hex colour, tolerating an optional leading <c>#</c> and both the shorthand
    /// (<c>RGB</c>) and full (<c>RRGGBB</c>) forms, without allocating or throwing on failure.
    /// </summary>
    private static bool TryParseValue(ReadOnlySpan<char> hexValue, out uint value)
    {
        value = 0;

        var trimmed = hexValue.Trim();

        // Drop a single leading '#'.
        if (trimmed.Length > 0 && trimmed[0] == '#')
            trimmed = trimmed[1..];

        Span<char> digits = stackalloc char[6];

        if (trimmed.Length == 3)
        {
            digits[0] = digits[1] = trimmed[0];
            digits[2] = digits[3] = trimmed[1];
            digits[4] = digits[5] = trimmed[2];
        }
        else if (trimmed.Length == 6)
        {
            trimmed.CopyTo(digits);
        }
        else
        {
            return false;
        }

        // Reject anything that isn't a hex digit; HexNumber otherwise tolerates surrounding
        // whitespace, which the original format did not.
        foreach (var c in digits)
        {
            if (!Uri.IsHexDigit(c)) return false;
        }

        return UInt32.TryParse(digits, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
    }

    /// <summary>
    /// Casts the <see cref="HexColour"/> to its hexadecimal string representation.
    /// </summary>
    /// <param name="hexColour">The <see cref="HexColour"/> instance to convert.</param>
    public static implicit operator string(HexColour hexColour) => hexColour.HexString;

    /// <summary>
    /// Casts a hexadecimal string representation to a <see cref="HexColour"/> instance.
    /// </summary>
    /// <param name="hexValue">The hexadecimal string to convert. Must be in the format "#RRGGBB" or "#RGB".</param>
    public static explicit operator HexColour(string hexValue) => new(hexValue);

    /// <summary>
    /// Casts the <see cref="HexColour"/> to its underlying unsigned integer value.
    /// </summary>
    /// <param name="hexColour">The <see cref="HexColour"/> instance to convert.</param>
    public static implicit operator uint(HexColour hexColour) => hexColour.Value;

    /// <summary>
    /// Casts an unsigned integer value to a <see cref="HexColour"/> instance.
    /// </summary>
    /// <param name="value"></param>
    public static explicit operator HexColour(uint value) => new(value);

    /// <summary>
    /// The string representation of the <see cref="HexColour"/> instance.
    /// </summary>
    /// <returns>The hexadecimal string representation in the format "#RRGGBB".</returns>
    public override string ToString() => HexString;

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="HexColour"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance. Must be a <see cref="HexColour"/> object.</param>
    /// <returns><see langword="true"/> if the specified object is a <see cref="HexColour"/> and is equal to the current
    /// instance; otherwise, <see langword="false"/>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is HexColour other && Equals(other);

    /// <summary>
    /// Compares if the current <see cref="HexColour"/> is equal to another.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(HexColour other) => _value == other._value;

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Compares if two <see cref="HexColour"/> instances are equal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><see langword="true"/> if both instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(HexColour left, HexColour right) => left.Equals(right);

    /// <summary>
    /// Compares if two <see cref="HexColour"/> instances are unequal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><see langword="true"/> if both instances are unequal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(HexColour left, HexColour right) => !left.Equals(right);
}

internal class HexColourJsonConverter : JsonConverter<HexColour>
{
    public override HexColour Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType != JsonTokenType.String) throw new JsonException($"Expected a string hex colour value but found token '{reader.TokenType}'.");

        var value = reader.GetString();
        if (String.IsNullOrEmpty(value)) return default;

        try
        {
            return new HexColour(value);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            throw new JsonException($"Could not parse '{value}' as a hex colour.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, HexColour value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.HexString);
    }
}