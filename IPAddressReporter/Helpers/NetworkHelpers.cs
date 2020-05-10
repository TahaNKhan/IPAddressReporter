using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace IPAddressReporter.Helpers
{
	public static class NetworkHelpers
	{
		public static bool IsNetworkConnected() => NetworkInterface.GetIsNetworkAvailable() && IsInternetAvailable();

		private static bool IsInternetAvailable() => new Ping().Send("www.google.com").Status == IPStatus.Success;

		public static bool IsVPNOn()
		{
			var isNetworkAvailable = IsNetworkConnected();
			var vpnNetworkExists = NetworkInterface.GetAllNetworkInterfaces()
				.Any(networkInterface =>
				{
					var isNetworkUp = networkInterface.OperationalStatus == OperationalStatus.Up;
					var isVpnNetwork = networkInterface.Description.ToLowerInvariant().Contains("vpn");
					return isNetworkUp && isVpnNetwork;
				});
			return vpnNetworkExists;
		}
	}
}
