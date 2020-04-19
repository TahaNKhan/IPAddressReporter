using IPAddressReporter.Logging;

namespace IPAddressReporter.Logic.Services.Interfaces
{
	public interface IServiceProxyFactory
	{
		IEmailService GetEmailService(ILogger logger);
		IDNSUpdateService GetDNSUpdateService(ILogger logger);
	}
}
