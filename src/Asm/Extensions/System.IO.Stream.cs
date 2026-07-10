namespace System.IO;

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
    /// This method supports passing of long values to <see cref="Stream.Read(global::System.Byte[], Int32, Int32)"/>.
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
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (offset + count > buffer.Length)
        {
            throw new ArgumentException("The sum of offset and count is larger than the buffer length.");
        }

        // A byte[] holds at most Int32.MaxValue elements, so any (offset, count) that fits the buffer
        // also fits Int32; delegate to the framework, preserving its (possibly partial) read semantics.
        return stream.Read(buffer, (int)offset, (int)count);
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
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (offset + count > buffer.Length)
        {
            throw new ArgumentException("The sum of offset and count is larger than the buffer length.");
        }

        // A byte[] holds at most Int32.MaxValue elements, so any (offset, count) that fits the buffer
        // also fits Int32.
        stream.Write(buffer, (int)offset, (int)count);
    }
}
