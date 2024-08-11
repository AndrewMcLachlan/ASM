namespace System.Net;

/// <summary>
/// Extensions for the <see cref="IPAddress"/> class.
/// </summary>
[CLSCompliant(false)]
public static class IPAddressExtensions
{
    /// <summary>
    /// Converts an IP address and subnet mask to a CIDR notation string.
    /// </summary>
    /// <param name="ipAddress">The IP address.</param>
    /// <param name="mask">The subnet mask.</param>
    /// <returns>The <paramref name="ipAddress"/> and <paramref name="mask"/> in CIDR notation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="ipAddress"/> or <paramref name="mask"/> are not IPv4 addresses.</exception>
    /// <exception cref="FormatException">Thrown if <paramref name="mask"/> is not a valid subnet mask.</exception>
    public static string ToCidrString(this IPAddress ipAddress, IPAddress mask)
    {
        if (ipAddress.AddressFamily != Sockets.AddressFamily.InterNetwork) throw new ArgumentException("Not an IPv4 address", nameof(ipAddress));
        if (mask.AddressFamily != Sockets.AddressFamily.InterNetwork) throw new ArgumentException("Not an IPv4 address", nameof(mask));

        uint bitCheck = 0b1000_0000_0000_0000_0000_0000_0000_0000;
        uint reverseCheck = 0b0000_0000_0000_0000_0000_0000_0000_0001;
        uint addressNumber = ipAddress.ToUInt32();
        uint maskNumber = mask.ToUInt32();

        byte cidrNumber = 0;

        for (int i = 31; i >= 0; i--)
        {
            if ((bitCheck & maskNumber) == bitCheck)
            {
                cidrNumber++;
                bitCheck >>= 1;
            }
            else
            {
                byte reverse = 0;
                for (int j = 0; j < 32; j++)
                {
                    if ((reverseCheck & maskNumber) == 0)
                    {
                        reverse++;
                        reverseCheck <<= 1;
                    }
                    else if (reverse + cidrNumber != 32)
                    {
                        throw new FormatException("Invalid mask");
                    }
                }
                break;
            }
        }

        uint byteMask = 0b1111_1111_0000_0000_0000_0000_0000_0000;

        var maskedAddress = (addressNumber & maskNumber);

        string newIp = String.Empty;

        for (int i = 0; i < 4; i++)
        {
            newIp += ((maskedAddress & byteMask) >> ((3 - i) * 8)).ToString() + ".";

            byteMask >>= 8;
        }

        newIp = newIp[0..^1];

        return newIp + "/" + cidrNumber.ToString();
    }

    /// <summary>
    /// Converts an IP address to a 32-bit unsigned integer.
    /// </summary>
    /// <param name="ipAddress">The IP address</param>
    /// <returns>The IP address as a 32-bit unsigned integer.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ipAddress"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="ipAddress"/> is not an IPv4 address.</exception>
    public static uint ToUInt32(this IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);
        if (ipAddress.AddressFamily != Sockets.AddressFamily.InterNetwork)
        {
            throw new ArgumentException("Not an IPv4 address", nameof(ipAddress));
        }

        var split = ipAddress.GetAddressBytes();
        uint result = 0;

        for (int i = 0, j = 24; i < split.Length; i++, j -= 8)
        {
            result += (uint)split[i] << j;
        }

        return result;
    }

    /// <summary>
    /// Converts a 32-bit unsigned integer to an IP address.
    /// </summary>
    /// <param name="address">The address as a 32-bit unsigned integer.</param>
    /// <returns>The IP address.</returns>
    public static IPAddress FromUInt32(uint address)
    {
        uint byteMask = 0b1111_1111_0000_0000_0000_0000_0000_0000;

        byte[] octets = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            octets[i] = (byte)((address & byteMask) >> ((3 - i) * 8));

            byteMask >>= 8;
        }

        return new IPAddress(octets);
    }
}
