using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IPAddressReporter.Extensions
{
    public static class StringNetworkExtensions
    {
        public static IPAddress ToIPAddress(this string ipAddress)
        {
            try
            {
                return IPAddress.Parse(ipAddress);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
