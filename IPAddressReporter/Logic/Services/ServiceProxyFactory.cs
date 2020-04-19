using IPAddressReporter.Configuration;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.Interfaces;

namespace IPAddressReporter.Logic.Services
{

	public class ServiceProxyFactory : IServiceProxyFactory
	{
		private readonly AppSettings _appSettings;
		public ServiceProxyFactory(AppSettings appSettings)
		{
			_appSettings = appSettings;
		}

		public IDNSUpdateService GetDNSUpdateService(ILogger logger)
		{
			return new GoDaddyDNSUpdateService.GoDaddyDNSUpdateService(_appSettings, logger);
		}

		public IEmailService GetEmailService(ILogger logger)
		{
			return new GmailService(_appSettings, logger);
		}
	}
}
