using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.Logic.Services.Interfaces
{
	public interface IDNSUpdateService
	{
		Task UpdateIPOnDNS(string host, IPAddress address, CancellationToken cancellationToken = default);
	}
}
