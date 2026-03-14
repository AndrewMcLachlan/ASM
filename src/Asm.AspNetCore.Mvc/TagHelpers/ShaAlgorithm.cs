namespace Asm.AspNetCore.Mvc.TagHelpers;

/// <summary>
/// Specifies the SHA algorithm used to compute the integrity hash.
/// </summary>
public enum ShaAlgorithm
{
    /// <summary>
    /// SHA-256.
    /// </summary>
    Sha256 = 256,

    /// <summary>
    /// SHA-384.
    /// </summary>
    Sha384 = 384,

    /// <summary>
    /// SHA-512.
    /// </summary>
    Sha512 = 512,
}
