using IPAddressReporter.Configuration;
using IPAddressReporter.DataAccess.Interfaces;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		private readonly IDataContextFactory _dataContextFactory;
		private readonly AppSettings _appSettings;

		private static IPAddress _currentIpAddress = null;

		public ReporterTask(IServiceProxyFactory serviceProxyFactory, IDataContextFactory dataContextFactory, AppSettings appSettings)
		{
			_serviceProxyFactory = serviceProxyFactory;
			_dataContextFactory = dataContextFactory;
			_appSettings = appSettings;
		}

		/// <summary>
		/// Looks up the external IP and reports if it has changed
		/// </summary>
		/// <returns>true if IP address changed</returns>
		public async Task<bool> ReportIPAddress(ILogger logger, CancellationToken cancellationToken = default)
		{
			if (_currentIpAddress == null)
				_currentIpAddress = await LoadIPAddress(logger);


			var ipAddress = await GetExternalIPAddressAsync(logger, cancellationToken);

			if (ipAddress.Equals(_currentIpAddress))
			{
				logger.LogInfo($"IP Address did not change, {ipAddress}");
				return false;
			}

			await SaveIPAddress(ipAddress, logger);

			logger.LogInfo($"IP changed, old IP: '{_currentIpAddress}', new IP: '{ipAddress}'");
			
			_currentIpAddress = ipAddress;

			await UpdateIPOnDNS(ipAddress, logger, cancellationToken);

			await SendIPAddressViaEmail(ipAddress, logger, cancellationToken);

			return true;
		}

		private async Task UpdateIPOnDNS(IPAddress address, ILogger logger, CancellationToken cancellationToken = default)
		{
			try
			{
				var dnsUpdateService = _serviceProxyFactory.GetDNSUpdateService(logger);
				await dnsUpdateService.UpdateIPOnDNS(_appSettings.Host, address, cancellationToken);
			}
			catch (Exception ex)
			{
				// Don't bomb if IP couldn't be updated.
				logger.LogError(ex.ToString());
			}
		}

		private async Task<IPAddress> LoadIPAddress(ILogger logger)
		{
			try
			{
				using var dataContext = _dataContextFactory.Construct();
				var ipDataAccess = dataContext.GetIPAddressDataAccess();
				return await ipDataAccess.LoadIPAddress(logger);
			}
			catch (Exception ex)
			{
				// Don't bomb when reading from file.
				logger.LogError($"Something bad happened when reading IP from file: {ex}");
			}
			return null;
		}

		internal virtual async Task SaveIPAddress(IPAddress ipAddress, ILogger logger)
		{
			try
			{
				using var dataContext = _dataContextFactory.Construct();
				var ipDataAccess = dataContext.GetIPAddressDataAccess();
				await ipDataAccess.SaveIPAddress(ipAddress, logger);
			}
			catch (Exception ioEx)
			{
				logger.LogError($"Failed to save IP address: {ioEx}");
			}
		}

		internal virtual async Task SendIPAddressViaEmail(IPAddress ipAddress, ILogger logger, CancellationToken cancellationToken = default)
		{
			const string subject = "Current IP Address";
			var to = _appSettings.RecipientEmails.ToList();
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
				throw new HttpRequestException();
			}

			var ipAddressString = await result.Content.ReadAsStringAsync();

			if (IPAddress.TryParse(ipAddressString.Trim(), out var ipAddress))
				return ipAddress;

			// Stop processing
			throw new Exception($"Unable to parse IP Address: {ipAddressString}");
		}
	}
}
