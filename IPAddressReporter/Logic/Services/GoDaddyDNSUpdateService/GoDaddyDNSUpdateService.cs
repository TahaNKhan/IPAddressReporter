﻿using IPAddressReporter.Configuration;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.GoDaddyDNSUpdateService.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.Logic.Services.GoDaddyDNSUpdateService
{
	public class GoDaddyDNSUpdateService : Interfaces.IDNSUpdateService
	{
		private readonly AppSettings _appSettings;
		private readonly ILogger _logger;

		public GoDaddyDNSUpdateService(AppSettings appSettings, ILogger logger)
		{
			_appSettings = appSettings;
			_logger = logger;
		}

		public async Task UpdateIPOnDNS(string host, IPAddress address, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(_appSettings?.GoDaddyAPISecrets?.URL))
				throw new Exception("GoDaddy API URL not setup");
			using var client = new HttpClient();
			
			AddAuthorizationHeader(client);

			const string type = "A";
			const string name = "@";
			var request = BuildDNSRecordRequest(address);
			
			var response = await client.PutAsync(_appSettings.GoDaddyAPISecrets.URL + $"/v1/domains/{host}/records/{type}/{name}", request, cancellationToken);
			
			if (response.IsSuccessStatusCode)
			{
				_logger.Log("Updated IP at GoDaddy successfully.");
				return;
			}

			_logger.Log("Failed to update IP at GoDaddy");
			_logger.Log($"Status Code: {response.StatusCode}");
			_logger.Log(await response.Content.ReadAsStringAsync());
		}

		private HttpContent BuildDNSRecordRequest(IPAddress address)
		{
			var request = new DNSRecordRequest
			{
				Data = address.ToString(),
				Port = 1,
				Priority = 0,
				Protocol = "",
				Service = "",
				TTL = 600,
				Weight = 1
			};
			var serializedRequest = JsonSerializer.Serialize(new[] { request }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
			var content = new System.Net.Http.StringContent(serializedRequest, Encoding.Default, "application/json");
			return content;
		}

		internal virtual void AddAuthorizationHeader(HttpClient client)
		{
			var apiKey = _appSettings.GoDaddyAPISecrets.APIKey;
			var apiSecret = _appSettings.GoDaddyAPISecrets.APISecret;

			client.DefaultRequestHeaders.Add("Authorization", $"sso-key {apiKey}:{apiSecret}");
		}
	}
}