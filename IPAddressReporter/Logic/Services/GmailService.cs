using IPAddressReporter.Configuration;
using IPAddressReporter.Exceptions;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.Logic.Services
{
	public class GmailService : IEmailService
	{
		private readonly Logging.ILogger _logger;
		private readonly AppSettings _appSettings;

		public GmailService(AppSettings appSettings, ILogger logger)
		{
			_logger = logger;
			_appSettings = appSettings;
		}

		public async Task SendEmailAsync(IList<string> recepients, string subject, string body, CancellationToken cancellationToken = default)
		{
			if (!recepients.Any())
				return;
			using var smtpClient = BuildSmtpClient();
			_logger.LogInfo($"Sending emails");
			var exceptions = new List<Exception>();
			foreach (var to in recepients)
			{
				try
				{
					_logger.LogInfo("Sent to: " + to);
					var mailMessage = GenerateMailMessage(to, subject, body);
					if (_appSettings.SendEmails)
						await smtpClient.SendMailAsync(mailMessage);
					else
						_logger.LogInfo($"Did not send email to {to} due to config settings");
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
					_logger.LogError($"Failed to send email to: {to}");
					_logger.LogError(ex.ToString());
					continue;
				}
			}
			if (exceptions.Any())
			{
				_logger.LogError("Unable to process emails, bubbling up exception");
				throw new CombinedException(exceptions);
			}
			else
			{
				_logger.LogInfo("Emails processed!");
			}
		}

		internal virtual MailMessage GenerateMailMessage(string to, string subject, string body)
		{
			var mailMessage = new MailMessage
			{
				From = new MailAddress(_appSettings.EmailCredentials.Email, _appSettings.EmailCredentials.EmailDisplayName),
				Body = body,
				Subject = subject
			};
			mailMessage.To.Add(new MailAddress(to));
			return mailMessage;
		}

		internal virtual SmtpClient BuildSmtpClient()
		{
			_logger.LogInfo("Connecting to smtp.gmail.com:587");
			var smtpClient = new SmtpClient("smtp.gmail.com", 587)
			{
				UseDefaultCredentials = true,
				Credentials = new NetworkCredential(_appSettings.EmailCredentials.Email, _appSettings.EmailCredentials.EmailPassword),
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network
			};
			_logger.LogInfo("Connected to gmail!");
			return smtpClient;
		}
	}
}