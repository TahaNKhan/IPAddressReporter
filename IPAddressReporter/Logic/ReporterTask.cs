﻿using IPAddressReporter.Configuration;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.Logic
{
	public interface IReporterTask
	{
		Task<bool> ReportIPAddress(CancellationToken cancellationToken = default);
	}

	public class ReporterTask : IReporterTask
	{
		private readonly IServiceProxyFactory _serviceProxyFactory;
		private readonly ILogger _logger;
		private readonly AppSettings _appSettings;

		private IPAddress _currentIpAddress;

		public ReporterTask(IServiceProxyFactory serviceProxyFactory, ILogger logger, AppSettings appSettings)
		{
			_currentIpAddress = null;
			_serviceProxyFactory = serviceProxyFactory;
			_logger = logger;
			_appSettings = appSettings;
		}

		/// <summary>
		/// Looks up the external IP and reports if it has changed
		/// </summary>
		/// <returns>true if IP address changed</returns>
		public async Task<bool> ReportIPAddress(CancellationToken cancellationToken = default)
		{
			var ipAddress = await GetExternalIPAddressAsync(cancellationToken);
			if (ipAddress.Equals(_currentIpAddress))
				return false;

			_currentIpAddress = ipAddress;

			await SendIPAddressViaEmail(ipAddress, cancellationToken);

			return true;
		}

		internal virtual async Task SendIPAddressViaEmail(IPAddress ipAddress, CancellationToken cancellationToken = default)
		{
			const string subject = "Current IP Address";
			var to = _appSettings.RecipientEmails;
			var body = $"Your current IP Address is {ipAddress}";
			var emailService = _serviceProxyFactory.GetEmailService();
			await emailService.SendEmailAsync(to, subject, body, cancellationToken);
		}

		internal virtual async Task<IPAddress> GetExternalIPAddressAsync(CancellationToken cancellationToken = default)
		{
			const string ipResolverServerUrl = "http://checkip.amazonaws.com/";
			using var webClient = new System.Net.Http.HttpClient();
			
			_logger.Log($"Sending GET request to {ipResolverServerUrl}");
			
			var result = await webClient.GetAsync(ipResolverServerUrl, cancellationToken);
			
			if (!result.IsSuccessStatusCode)
			{
				_logger.Log("Failed to obtain external IP address");
				_logger.Log($"Status code: {result.StatusCode}");
				_logger.Log($"Response: {await result.Content?.ReadAsStringAsync()}");
				throw new Exception();
			}

			var ipAddressString = await result.Content.ReadAsStringAsync();

			if (IPAddress.TryParse(ipAddressString.Trim(), out var ipAddress))
				return ipAddress;

			_logger.Log($"Unable to parse IP Address: {ipAddressString}");
			throw new Exception();
		}
	}
}