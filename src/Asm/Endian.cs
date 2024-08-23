namespace Asm;

/// <summary>
/// Indicates binary endianness.
/// </summary>
public enum Endian
{
    /// <summary>
    /// The first bit is the least significant.
    /// </summary>
    BigEndian,
    /// <summary>
    /// The last bit is the least significant.
    /// </summary>
    LittleEndian
}