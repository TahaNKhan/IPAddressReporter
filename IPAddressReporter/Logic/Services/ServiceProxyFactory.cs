using IPAddressReporter.Configuration;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.Interfaces;

namespace IPAddressReporter.Logic.Services
{

	public class ServiceProxyFactory : IServiceProxyFactory
	{
		private readonly AppSettings _appSettings;
		private readonly ILogger _logger;
		public ServiceProxyFactory(AppSettings appSettings, ILogger logger)
		{
			_appSettings = appSettings;
			_logger = logger;
		}

		public IDNSUpdateService GetDNSUpdateService()
		{
			return new GoDaddyDNSUpdateService.GoDaddyDNSUpdateService(_appSettings, _logger);
		}

		public IEmailService GetEmailService()
		{
			return new GmailService(_appSettings, _logger);
		}
	}
}
