using IPAddressReporter.Configuration;
using IPAddressReporter.DataAccess;
using IPAddressReporter.DataAccess.Interfaces;
using IPAddressReporter.Exceptions;
using IPAddressReporter.Extensions;
using IPAddressReporter.Helpers;
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

        private static IPReporterState _ipReporterState = null;

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
            if (!await ShouldIPAddressBeUpdated(logger))
                return false;

            if (_ipReporterState == null)
                _ipReporterState = await LoadState(logger);

            var ipAddress = await GetExternalIPAddressAsync(logger, cancellationToken);

            var ipAddressChanged = ipAddress == null || !ipAddress.Equals(_ipReporterState?.IPAddress.ToIPAddress());

            if (ipAddressChanged)
            {
                logger.LogInfo($"IP changed, old IP: '{_ipReporterState?.IPAddress}', new IP: '{ipAddress?.ToString()}'");
            }
            else
            {
                logger.LogInfo($"IP Address did not change, {ipAddress}");
            }

            var updatedDNS = _ipReporterState?.IsDNSUpdated ?? false;
            var sentNotificaiton = _ipReporterState?.IsIPNotificationSent ?? false;

            if (!updatedDNS || ipAddressChanged)
                updatedDNS = await UpdateIPOnDNS(ipAddress, logger, cancellationToken);

            if (!sentNotificaiton || ipAddressChanged)
                sentNotificaiton = await SendIPAddressViaEmail(ipAddress, logger, cancellationToken);

            var state = new IPReporterState
            {
                IPAddress = ipAddress.ToString(),
                IsDNSUpdated = updatedDNS,
                IsIPNotificationSent = sentNotificaiton
            };

            await SaveIPReporterState(state, logger);

            return true;
        }

        /// <summary>
        /// Updates IP on DNS server
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        private async Task<bool> UpdateIPOnDNS(IPAddress address, ILogger logger, CancellationToken cancellationToken = default)
        {
            try
            {
                var dnsUpdateService = _serviceProxyFactory.GetDNSUpdateService(logger);
                await dnsUpdateService.UpdateIPOnDNS(_appSettings.Host, address, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                // Don't bomb if IP couldn't be updated.
                logger.LogError(ex.ToString());
                return false;
            }
        }

        private async Task<bool> ShouldIPAddressBeUpdated(ILogger logger)
        {
            if (!await NetworkHelpers.IsNetworkConnected())
            {
                logger.LogError("No connection to the internet");
                return false;
            }

            if (NetworkHelpers.IsVPNOn())
            {
                logger.LogInfo("VPN is on, IP address will not be updated.");
                return false;
            }
            return true;
        }

        private async Task<IPReporterState> LoadState(ILogger logger)
        {
            try
            {
                using var dataContext = _dataContextFactory.Construct();
                var ipDataAccess = dataContext.GetIPAddressDataAccess();
                return await ipDataAccess.LoadIPReporterState(logger);
            }
            catch (Exception ex)
            {
                // Don't bomb when reading from file.
                logger.LogError($"Something bad happened when reading IP from file: {ex}");
            }
            return null;
        }

        internal virtual async Task SaveIPReporterState(IPReporterState ipReporterState, ILogger logger)
        {
            try
            {
                using var dataContext = _dataContextFactory.Construct();
                var ipDataAccess = dataContext.GetIPAddressDataAccess();
                await ipDataAccess.SaveIPAddress(ipReporterState, logger);
            }
            catch (Exception ioEx)
            {
                logger.LogError($"Failed to save IP address: {ioEx}");
            }
        }

        internal virtual async Task<bool> SendIPAddressViaEmail(IPAddress ipAddress, ILogger logger, CancellationToken cancellationToken = default)
        {
            const string subject = "Current IP Address";
            var to = _appSettings.RecipientEmails.ToList();
            var body = $"Your current IP Address is {ipAddress}";
            try
            {
                var emailService = _serviceProxyFactory.GetEmailService(logger);
                await emailService.SendEmailAsync(to, subject, body, cancellationToken);
                return true;
            } catch (CombinedException ex)
            {
                logger.LogError(ex.ToString());
                return false;
            }
        }

        internal virtual async Task<IPAddress> GetExternalIPAddressAsync(ILogger logger, CancellationToken cancellationToken = default)
        {
            const string ipResolverServerUrl = "http://checkip.amazonaws.com/";
            using var webClient = new HttpClient();

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
