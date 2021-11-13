using System;
using System.Collections;

namespace Asm.Media
{
	/// <summary>
	/// Summary description for QuicktimeAtom.
	/// </summary>
	public class QuicktimeAtom
	{
		#region Fields
		//private int id;
		private ushort headerSize;
		private ArrayList atoms;
		private ulong size;
		private string type;
		private QuicktimeAtom parent;
		#endregion

		#region Properties
		/*public int Id
		{
			get
			{
				return id;
			}
		}
*/
        /// <summary>
        /// The atom type.
        /// </summary>
		public string MediaType
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

        /// <summary>
        /// The size.
        /// </summary>
        [CLSCompliant(false)]
		public ulong Size
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
        /// The header size in bytes.
        /// </summary>
        [CLSCompliant(false)]
		public ushort HeaderSize
		{
			get
			{
				return headerSize;
			}
			set
			{
				headerSize = value;
			}
		}

        /// <summary>
        /// Child atoms.
        /// </summary>
		public ArrayList Atoms
		{
			get
			{
				return atoms;
			}
		}

        /// <summary>
        /// Parent atom.
        /// </summary>
		public QuicktimeAtom Parent
		{
			get
			{
				return parent;
			}
		}
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QuicktimeAtom"/> class.
        /// </summary>
		public QuicktimeAtom()
		{
			this.atoms = new ArrayList();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="QuicktimeAtom"/> class.
        /// </summary>
        /// <param name="parent">The atom's parent.</param>
		public QuicktimeAtom(QuicktimeAtom parent)
		{
			this.atoms = new ArrayList();
			this.parent = parent;
		}
		#endregion
	}
}
