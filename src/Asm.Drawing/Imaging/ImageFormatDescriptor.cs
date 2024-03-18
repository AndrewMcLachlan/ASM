using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Asm.Drawing.Imaging;

/// <summary>
/// Describes an image format.
/// </summary>
/// <remarks>
/// TODO: Add MIME type field and static default implementations.
/// </remarks>
public class ImageFormatDescriptor
{
    #region Properties
    /// <summary>
    /// Gets a string describing the image format.
    /// </summary>
    /// <example>"Windows Bitmap" for bitmap files.</example>
    public string Description { get; private set; }

    /// <summary>
    /// Gets a short string describing the image format.
    /// </summary>
    /// /// <example>"BMP" for bitmap files.</example>
    public string ShortForm { get; private set; }

    /// <summary>
    /// Gets a list of extensions commonly associated with this image type.
    /// </summary>
    public ReadOnlyCollection<string> Extensions { get; private set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFormatDescriptor" /> class.
    /// </summary>
    /// <param name="description">The image format's description.</param>
    /// <param name="shortForm">The short form of the image format's name.</param>
    /// <param name="extensions">Extensions associated with this image type.</param>
    public ImageFormatDescriptor(string description, string shortForm, string[] extensions)
    {
        Description = description;
        ShortForm = shortForm;
        Extensions = new ReadOnlyCollection<string>(extensions);
    }
    #endregion
}