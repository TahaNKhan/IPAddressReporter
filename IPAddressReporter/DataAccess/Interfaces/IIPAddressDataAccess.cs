using IPAddressReporter.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.DataAccess.Interfaces
{
	public interface IIPAddressDataAccess
	{
		Task<IPAddress> LoadIPAddress(ILogger logger = null);
		Task SaveIPAddress(IPAddress address, ILogger logger = null);
	}
}
