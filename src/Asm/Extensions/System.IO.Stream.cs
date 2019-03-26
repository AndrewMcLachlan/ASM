using System;
using System.IO;

namespace Asm.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="System.IO.Stream"/> class.
    /// </summary>
    /// <remarks>
    /// Need to implement the asynchronous version of the Read and Write methods.
    /// </remarks>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads a sequence of bytes from the current
        /// stream and advances the position within the stream by the number of bytes
        /// read.
        /// </summary>
        /// <remarks>
        /// This method supports passing of long values to <see cref="Stream.Read"/>.
        /// </remarks>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced
        /// by the bytes read from the current source.</param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read
        /// from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// The total number of bytes read into the buffer. This can be less than the
        /// number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the
        /// umber of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="System.ArgumentException">The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="System.ArgumentNullException">buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">offset or count is negative.</exception>
        /// <exception cref=" System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public static long Read(this Stream stream, byte[] buffer, long offset, long count)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (count <= Int32.MaxValue && offset <= Int32.MaxValue)
            {
                return stream.Read(buffer, (int)offset, (int)count);
            }
            else
            {
                int segment = Int32.MaxValue;
                long innerOffset = offset;
                long bytesRead = 0;
                while ((count - segment) > 0)
                {
                    byte[] innerBuffer = new byte[segment];

                    bytesRead += stream.Read(innerBuffer, 0, segment);
                    innerBuffer.CopyTo(buffer, innerOffset);

                    innerOffset += segment;
                    segment = (count - segment) > Int32.MaxValue ? Int32.MaxValue : checked((int)(count - segment));
                }

                return bytesRead;
            }
        }

        /// <summary>
        /// Writes a sequence of bytes to the current
        /// stream and advances the current position within this stream by the number
        /// of bytes written.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes from buffer to the current
        /// stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin copying bytes to the
        /// current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// The sum of offset and count is greater than the buffer length.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">buffer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">offset or count is negative.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="System.NotSupportedException">The stream does not support writing.</exception>
        /// <exception cref="System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public static void Write(this Stream stream, byte[] buffer, long offset, long count)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (count <= Int32.MaxValue && offset <= Int32.MaxValue)
            {
                stream.Write(buffer, (int)offset, (int)count);
            }
            else
            {
                int segment = Int32.MaxValue;
                long innerOffset = offset;
                while ((count - segment) > 0)
                {
                    byte[] innerBuffer = new byte[segment];

                    Array.Copy(buffer, innerOffset, innerBuffer, 0, segment);

                    stream.Write(innerBuffer, 0, segment);
                    innerOffset += segment;
                    segment = (count - segment) > Int32.MaxValue ? Int32.MaxValue : checked((int)(count - segment));
                }
            }
        }
    }
}
