using System;

namespace Asm.Drawing
{
    /// <summary>
    /// The orientation of an image.
    /// </summary>
    /// <remarks>
    /// The values describe the how the image has been altered from its "normal" position.
    /// </remarks>
    public enum ExifOrientation
    {
        /// <summary>
        /// Value not set.
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// Normal rotation.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The image has been flipped horizontally.
        /// </summary>
        FlippedHorizontal = 2,
        /// <summary>
        /// The image has been rotated 180 degrees;
        /// </summary>
        Rotated180 = 3,
        /// <summary>
        /// The image has been flipped horizontally and rotated 180 degrees.
        /// </summary>
        FlipedHorizonalRotated180 = 4,
        /// <summary>
        /// The image has been flipped horizontally and rotated 90 degrees clockwise.
        /// </summary>
        FlippedHorizonalRotated90 = 5,
        /// <summary>
        /// The image has been rotated 270 degrees clockwise.
        /// </summary>
        Rotated270 = 6,
        /// <summary>
        /// The image has been flipped horizontally and rotated 270 degrees clockwise.
        /// </summary>
        FlippedHorizonalRotated270 = 7,
        /// <summary>
        /// The has been rotated 90 degrees clockwise.
        /// </summary>
        Rotated90 = 8,
    }
}
