using IPAddressReporter.Configuration;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.Interfaces;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.Logic
{
	public interface IReporterTask
	{
		Task<bool> ReportIPAddress(ILogger logger, CancellationToken cancellationToken = default);
	}

	public class ReporterTask : IReporterTask
	{
		private readonly IServiceProxyFactory _serviceProxyFactory;
		private readonly AppSettings _appSettings;

		private static IPAddress _currentIpAddress = null;

		public ReporterTask(IServiceProxyFactory serviceProxyFactory, ILoggerFactory loggerFactory, AppSettings appSettings)
		{
			_serviceProxyFactory = serviceProxyFactory;
			_appSettings = appSettings;
		}

		/// <summary>
		/// Looks up the external IP and reports if it has changed
		/// </summary>
		/// <returns>true if IP address changed</returns>
		public async Task<bool> ReportIPAddress(ILogger logger, CancellationToken cancellationToken = default)
		{
			var ipAddress = await GetExternalIPAddressAsync(logger, cancellationToken);

			if (ipAddress.Equals(_currentIpAddress))
			{
				logger.LogInfo($"IP Address did not change, {ipAddress}");
				return false;
			}

			logger.LogInfo($"IP changed, old IP: '{_currentIpAddress}', new IP: '{ipAddress}'");
			_currentIpAddress = ipAddress;

			try
			{
				var dnsUpdateService = _serviceProxyFactory.GetDNSUpdateService(logger);
				await dnsUpdateService.UpdateIPOnDNS(_appSettings.Host, ipAddress, cancellationToken);
			}
			catch(Exception ex)
			{
				// Don't bomb if ip couldn't be updated.
				logger.LogError(ex.ToString());
			}

			await SendIPAddressViaEmail(ipAddress, logger, cancellationToken);

			return true;
		}

		internal virtual async Task SendIPAddressViaEmail(IPAddress ipAddress, ILogger logger, CancellationToken cancellationToken = default)
		{
			const string subject = "Current IP Address";
			var to = _appSettings.RecipientEmails;
			var body = $"Your current IP Address is {ipAddress}";
			var emailService = _serviceProxyFactory.GetEmailService(logger);
			await emailService.SendEmailAsync(to, subject, body, cancellationToken);
		}

		internal virtual async Task<IPAddress> GetExternalIPAddressAsync(ILogger logger, CancellationToken cancellationToken = default)
		{
			const string ipResolverServerUrl = "http://checkip.amazonaws.com/";
			using var webClient = new System.Net.Http.HttpClient();

			logger.LogInfo($"Sending GET request to {ipResolverServerUrl}");

			var result = await webClient.GetAsync(ipResolverServerUrl, cancellationToken);

			if (!result.IsSuccessStatusCode)
			{
				logger.LogError("Failed to obtain external IP address");
				logger.LogError($"Status code: {result.StatusCode}");
				logger.LogError($"Response: {await result.Content?.ReadAsStringAsync()}");
				throw new Exception();
			}

			var ipAddressString = await result.Content.ReadAsStringAsync();

			if (IPAddress.TryParse(ipAddressString.Trim(), out var ipAddress))
				return ipAddress;

			// Stop processing
			throw new Exception($"Unable to parse IP Address: {ipAddressString}");
		}
	}
}
