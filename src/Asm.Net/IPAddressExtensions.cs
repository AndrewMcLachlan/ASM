using System;
using System.Net;

namespace Asm.Net
{
    [CLSCompliant(false)]
    public static class IPAddressExtensions
    {
        public static string ToCidrString(this IPAddress ipAddress, IPAddress mask)
        {
            if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) throw new InvalidOperationException("Not an IPv4 address");

            uint bitCheck = 0b1000_0000_0000_0000_0000_0000_0000_0000;
            uint reverseCheck = 0b0000_0000_0000_0000_0000_0000_0000_0001;
            uint addressNumber = ipAddress.ToUInt32();
            uint maskNumber = mask.ToUInt32();

            byte cidrNumber = 0;

            int index = 0;
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
                            throw new InvalidCastException("Invalid mask");
                        }
                    }
                    break;
                }

                index++;
            }

            uint byteMask = 0b1111_1111_0000_0000_0000_0000_0000_0000;

            var maskedAddress = (addressNumber & maskNumber);

            string newIp = String.Empty;

            for (int i = 0; i < 4; i++)
            {
                newIp += ((maskedAddress & byteMask) >> ((3 - i) * 8)).ToString() + ".";

                byteMask >>= 8;
            }

            newIp = newIp.Substring(0, newIp.Length - 1);

            return newIp + "/" + cidrNumber.ToString();
        }

        public static uint ToUInt32(this IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));
            if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentException("Not a valid IPv4 Address", nameof(ipAddress));
            }

            var split = ipAddress.GetAddressBytes();
            uint result = 0;

            for (int i = 0, j = 24; i < split.Length; i++, j -= 8)
            {
                result += (uint)split[i] << j;
            }

            return result;
        }

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
}
