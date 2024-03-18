using System;
using System.Collections.ObjectModel;

namespace Asm.Media;

/// <summary>
/// Generic class that represents a header object in a movie file.
/// </summary>
public sealed class Header
{
    #region Fields
    private short headerSize;
    private Collection<Header> subHeaders;
    private long size;
    private string type;
    private short version;
    private Header parent;
    #endregion

    #region Properties
    /// <summary>
    /// The type identifier for the header object. The format of this string is dependent on the type of movie file.
    /// </summary>
    public string MediaType
    {
        get { return type; }
        set { type = value; }
    }

    /// <summary>
    /// The size of the data described by the header in bytes.
    /// </summary>
    public long Size
    {
        get { return size; }
        set { size = value; }
    }

    /// <summary>
    /// The version (if any) of the header.
    /// </summary>
    public short Version
    {
        get { return version; }
        set { version = value; }
    }

    /// <summary>
    /// The size of the header itself in bytes.
    /// </summary>
    public short HeaderSize
    {
        get { return headerSize; }
        set { headerSize = value; }
    }

    /// <summary>
    /// A collection of Header objects that are contained in this object.
    /// </summary>
    public Collection<Header> SubHeaders
    {
        get { return subHeaders; }
    }

    /// <summary>
    /// The parent object to which this Header belongs.
    /// </summary>
    public Header Parent
    {
        get { return parent; }
    }


    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new instance of the <see cref="Header"/> class.
    /// </summary>
    public Header()
    {
        this.subHeaders = new Collection<Header>();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Header"/> class.
    /// </summary>
    /// <param name="parent">The <see cref="Header"/> to which this object belongs.</param>
    public Header(Header parent)
    {
        this.subHeaders = new Collection<Header>();
        this.parent = parent;
    }
    #endregion
}
