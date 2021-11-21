using System;
using System.Collections;

namespace Asm.Media
{
	/// <summary>
	/// Summary description for RealMediaObject.
	/// </summary>
	public class RealMediaObject
	{
		#region Fields
		private string objectId;
		private uint size;
		private ushort version;
		private RealMediaObject parent = null;
		private readonly ArrayList realMediaObjects;
		#endregion

		#region Properties
        /// <summary>
        /// The object ID.
        /// </summary>
		public string ObjectId
		{
			get
			{
				return objectId;
			}
			set
			{
				objectId = value;
			}
		}

        /// <summary>
        /// The size.
        /// </summary>
        [CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
			}
		}

        /// <summary>
        /// The version.
        /// </summary>
        [CLSCompliant(false)]
		public ushort Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

        /// <summary>
        /// The parent object.
        /// </summary>
		public RealMediaObject Parent
		{
			get
			{
				return parent;
			}
		}

        /// <summary>
        /// The child objects.
        /// </summary>
		public ArrayList RealMediaObjects
		{
			get
			{
				return realMediaObjects;
			}
		}
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RealMediaObject"/> class.
        /// </summary>
		public RealMediaObject()
		{
			realMediaObjects = new ArrayList();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="RealMediaObject"/> class.
        /// </summary>
        /// <param name="parent">The object's parent object.</param>
		public RealMediaObject(RealMediaObject parent)
		{
			this.parent = parent;
			realMediaObjects = new ArrayList();
		}
		#endregion
	}
}
