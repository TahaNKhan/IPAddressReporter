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
				var isVpnNetwork = IsVPNNetworkName(networkInterface.Description);
				return isNetworkUp && isVpnNetwork;
			}
			var vpnNetworkExists = NetworkInterface.GetAllNetworkInterfaces().Any(isNetworkVpnAndUp);
			return vpnNetworkExists;
		}

		private static bool IsVPNNetworkName(string networkInterfaceName)
		{
			var vpnNameList = new HashSet<string>
			{
				"vpn",
				"nord",
				"surfshark"
			};

			return vpnNameList.Contains(networkInterfaceName.ToLowerInvariant());
		}
	}
}
