using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Tools.Net.Scanner
{
    public static class NetworkCalculator
    {
        public static IEnumerable<string> GetUsableIps(UnicastIPAddressInformation ipInfo)
        {
            byte[] ipBytes = ipInfo.Address.GetAddressBytes();
            byte[] maskBytes = ipInfo.IPv4Mask.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ipBytes);
                Array.Reverse(maskBytes);
            }

            uint ip = BitConverter.ToUInt32(ipBytes, 0);
            uint mask = BitConverter.ToUInt32(maskBytes, 0);

            uint network = ip & mask;
            uint broadcast = network | ~mask;

            for (uint i = network + 1; i < broadcast; i++)
            {
                byte[] bytes = BitConverter.GetBytes(i);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }
                yield return new IPAddress(bytes).ToString();
            }
        }
    }
}