using IPAddressReporter.Configuration;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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

		public IEmailService GetEmailService()
		{
			return new GmailService(_appSettings, _logger);
		}
	}
}
