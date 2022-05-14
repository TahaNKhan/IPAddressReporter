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
		Task<IPReporterState> LoadIPReporterState(ILogger logger = default);
		Task SaveIPAddress(IPReporterState ipReporterState, ILogger logger = default);
	}
}
