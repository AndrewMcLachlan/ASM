using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Asm.Win32
{
    /// <summary>
    /// An entry in the hosts file.
    /// </summary>
    public class HostEntry
    {
        #region Constants
        private const string CommentChar = "#";
        #endregion

        #region Fields
        private bool _isCommented;
        #endregion

        #region Properties
        /// <summary>
        /// The IP address of the entry.
        /// </summary>
        public IPAddress? Address { get; set; }

        /// <summary>
        /// The alias assigned to the IP address.
        /// </summary>
        public string? Alias { get; set; }

        /// <summary>
        /// Any comment attached to the end of the IP address.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// The type of entry.
        /// </summary>
        public HostEntryType EntryType
        {
            get
            {
                //if (Address == null && !String.IsNullOrEmpty(Alias)) throw new ArgumentException();
                //else if (Address != null && String.IsNullOrEmpty(Alias)) throw new ArgumentException();
                //else if (Address == null && String.IsNullOrEmpty(Alias) && String.IsNullOrEmpty(comment)) EntryType = HostEntryType.Blank;
                if (Address == null && String.IsNullOrEmpty(Alias) && IsCommented) return HostEntryType.Comment;
                else if (Address != null && !String.IsNullOrEmpty(Alias) && !IsCommented)
                {
                    return Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? HostEntryType.IPv4Entry : HostEntryType.IPv6Entry;
                }
                else if (Address != null && !String.IsNullOrEmpty(Alias) && IsCommented)
                {
                    return Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? HostEntryType.IPv4EntryCommented : HostEntryType.IPv6EntryCommented;
                }
                else return HostEntryType.Blank;
            }
        }

        /// <summary>
        /// Gets or sets a value indicated if this entry commented out.
        /// </summary>
        /// <remarks>
        /// Returns false for valid entries with an appended comment.
        /// </remarks>
        public bool IsCommented
        {
            get { return _isCommented; }
            set
            {
                /*if (!value && (Address == null || String.IsNullOrEmpty(Alias)))
                {
                    throw new InvalidOperationException();
                }*/
                _isCommented = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HostEntry"/> class.
        /// </summary>
        /// <remarks>
        /// Used for constructing blank lines.
        /// </remarks>
        public HostEntry() : this(null, null, null, false)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostEntry"/> class.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <remarks>
        /// Used for constructing comment entries.
        /// </remarks>
        public HostEntry(string? comment) : this(null, null, comment, true)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostEntry"/> class.
        /// </summary>
        /// <param name="address">The IP address of the line.</param>
        /// <param name="alias">The alias assigned to the address.</param>
        /// <param name="comment">Any appended comment.</param>
        /// <param name="isCommented">Whether or not this line is commented.</param>
        public HostEntry(IPAddress? address, string? alias, string? comment, bool isCommented)
        {
            Address = address;
            Alias = alias;
            Comment = comment;
            IsCommented = isCommented;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts this entry to its string representation.
        /// </summary>
        /// <returns>The string representation of the entry, as it appears in the hosts file.</returns>
        public override string ToString()
        {
            string format = String.Empty;

            if (IsCommented)
            {
                format += CommentChar;
            }

            switch (EntryType)
            {
                case HostEntryType.IPv4Entry:
                case HostEntryType.IPv4EntryCommented:
                case HostEntryType.IPv6Entry:
                case HostEntryType.IPv6EntryCommented:
                    format += String.IsNullOrEmpty(Comment) ? "{0}\t{1}" : "{0}\t{1} {2}{3}";
                    break;
                case HostEntryType.Comment:
                    format += "{3}";
                    break;
                case HostEntryType.Blank:
                    break;
                default:
                    format = String.Empty;
                    break;
            }

            return String.Format(format, Address, Alias, CommentChar, Comment);
        }
        #endregion
    }

    /// <summary>
    /// Types of host entry.
    /// </summary>
    public enum HostEntryType
    {
        /// <summary>
        /// A blank line.
        /// </summary>
        Blank = 0,
        /// <summary>
        /// An IPv4 entry.
        /// </summary>
        IPv4Entry = 1,
        /// <summary>
        /// An IPv6 entry.
        /// </summary>
        IPv6Entry = 2,
        /// <summary>
        /// A comment line.
        /// </summary>
        Comment = 3,
        /// <summary>
        /// An IPv4 entry which is commented out.
        /// </summary>
        IPv4EntryCommented = 4,
        /// <summary>
        /// An IPv6 entry which is commented out.
        /// </summary>
        IPv6EntryCommented = 5,
    }
}
