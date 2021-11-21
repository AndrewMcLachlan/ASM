using System.Collections;

namespace Asm.Media;

/// <summary>
/// Summary description for RealMediaObject.
/// </summary>
public class RealMediaObject
{
    #region Properties
    /// <summary>
    /// The object ID.
    /// </summary>
    public string? ObjectId { get; set; }

    /// <summary>
    /// The size.
    /// </summary>
    [CLSCompliant(false)]
    public uint Size { get; set; }

    /// <summary>
    /// The version.
    /// </summary>
    [CLSCompliant(false)]
    public ushort Version { get; set; }

    /// <summary>
    /// The parent object.
    /// </summary>
    public RealMediaObject? Parent { get; }

    /// <summary>
    /// The child objects.
    /// </summary>
    public ArrayList RealMediaObjects { get; } = new ArrayList();
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RealMediaObject"/> class.
    /// </summary>
    public RealMediaObject()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RealMediaObject"/> class.
    /// </summary>
    /// <param name="parent">The object's parent object.</param>
    public RealMediaObject(RealMediaObject parent)
    {
        Parent = parent;
    }
    #endregion
}
