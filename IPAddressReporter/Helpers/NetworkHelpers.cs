using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressReporter.Helpers
{
	public static class NetworkHelpers
	{
		public static async Task<bool> IsNetworkConnected() => NetworkInterface.GetIsNetworkAvailable() && await IsInternetAvailable();

		private static async Task<bool> IsInternetAvailable() {
			using var ping = new Ping();
			var pingReply = await ping.SendPingAsync("www.google.com");
			return pingReply.Status == IPStatus.Success;
		}

		public static bool IsVPNOn()
		{
			static bool isNetworkVpnAndUp(NetworkInterface networkInterface)
			{
				var isNetworkUp = networkInterface.OperationalStatus == OperationalStatus.Up;
				var isVpnNetwork = networkInterface.Description.ToLowerInvariant().Contains("vpn");
				return isNetworkUp && isVpnNetwork;
			}
			var isNetworkAvailable = IsNetworkConnected();
			var vpnNetworkExists = NetworkInterface.GetAllNetworkInterfaces().Any(isNetworkVpnAndUp);
			return vpnNetworkExists;
		}
	}
}
