using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace FranceJudo.Core.Network.Scanner
{
    public static class NetworkCalculator
    {
        // Conserve la signature d'origine pour ne pas casser ton code appelant
        public static IEnumerable<string> GetUsableIps(UnicastIPAddressInformation ipInfo)
        {
            if (ipInfo == null || ipInfo.Address == null || ipInfo.IPv4Mask == null)
                yield break; // Sécurité anti-crash : Certains adaptateurs n'ont pas de masque

            foreach (var ip in GetUsableIps(ipInfo.Address, ipInfo.IPv4Mask))
            {
                yield return ip;
            }
        }

        // NOUVELLE METHODE 100% Testable
        public static IEnumerable<string> GetUsableIps(IPAddress ipAddress, IPAddress ipv4Mask)
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = ipv4Mask.GetAddressBytes();

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