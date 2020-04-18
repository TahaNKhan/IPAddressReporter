using System.Collections.Generic;

namespace IPAddressReporter.Configuration
{
	public class AppSettings
	{
		public IEnumerable<string> RecipientEmails { get; set; }
		public string LogFileLocation { get; set; }
		public bool SendEmails { get; set; }
		public EmailCredentials EmailCredentials { get; set; }
		public double WaitTimeSeconds { get; set; }
		public string Host { get; set; }
		public APISecrets GoDaddyAPISecrets { get; set; }
	}
	public class EmailCredentials
	{
		public string Email { get; set; }
		public string EmailPassword { get; set; }
		public string EmailDisplayName { get; set; }
	}

	public class APISecrets
	{
		public string URL { get; set; }
		public string APIKey { get; set; }
		public string APISecret { get; set; }
	}
}
