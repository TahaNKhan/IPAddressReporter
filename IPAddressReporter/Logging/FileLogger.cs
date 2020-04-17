using IPAddressReporter.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IPAddressReporter.Logging
{
	public class FileLogger : ILogger
	{
		private readonly StringBuilder _logs;
		private readonly string _logFileLocation;
		public const string DefaultLogFileLocation = "D:\\Logs\\IPAddressReporter.log";

		public FileLogger(AppSettings appSettings)
		{
			_logFileLocation = !string.IsNullOrEmpty(appSettings?.LogFileLocation) ? appSettings.LogFileLocation : DefaultLogFileLocation;
			_logs = new StringBuilder();
		}

		public void Log(string str)
		{
			str = str.Replace(Environment.NewLine, " ");
			str = str.Replace("\n", " ");
			str = str.Replace("\r", " ");
			_logs.Append($"{DateTimeOffset.Now:MMM dd yyyy HH:mm:ss zzz} - {str}{Environment.NewLine}");
		}

		public void Publish()
		{
			File.AppendAllText(_logFileLocation, _logs.ToString());
		}
	}
}
