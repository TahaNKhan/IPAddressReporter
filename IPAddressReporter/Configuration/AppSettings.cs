using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.Configuration
{
	public class AppSettings
	{
		public IEnumerable<string> RecipientEmails { get; set; }
		public string LogFileLocation { get; set; }
		public bool SendEmails { get; set; }
		public EmailCredentials EmailCredentials { get; set; }
		public double WaitTimeSeconds { get; set; }
	}
	public class EmailCredentials
	{
		public string Email { get; set; }
		public string EmailPassword { get; set; }
		public string EmailDisplayName { get; set; }
	}
}
