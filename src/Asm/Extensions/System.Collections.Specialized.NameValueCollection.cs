using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Asm.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="System.Collections.Specialized.NameValueCollection"/> class.
    /// </summary>
	public static class NameValueCollectionExtensions
	{
        /// <summary>
        /// Gets a value of a given type or a default if the key is not in the collection.
        /// </summary>
        /// <remarks>
        /// This is optimistic and will return the default value if any type conversion errors occur.
        /// </remarks>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name of the key.</param>
        /// <param name="defaultValue">The default value to retrieve if the key does not exist.</param>
        /// <returns>If the key exists the value in the given type is returned. Otherwise the <paramref name="defaultValue"/> is returned.</returns>
        public static T GetValue<T>(this NameValueCollection collection, string name, T defaultValue)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            if (collection[name] == null)
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(collection[name], typeof(T));
            }
            catch(InvalidCastException)
            {
                return defaultValue;
            }
            catch(FormatException)
            {
                return defaultValue;
            }
            catch(OverflowException)
            {
                return defaultValue;
            }
        }
	}
}
