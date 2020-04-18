using IPAddressReporter.Configuration;
using System;
using System.IO;
using System.Text;

namespace IPAddressReporter.Logging
{
	public class FileLogger : ILogger
	{
		private readonly StringBuilder _logs;
		private readonly string _logFileLocation;

		public FileLogger(AppSettings appSettings)
		{
			_logFileLocation = appSettings?.LogFileLocation;
			_logs = new StringBuilder();
		}

		public void Log(string str)
		{
			_logs.Append(ILogger.FormatLog(str));
		}

		public void Publish()
		{
			File.AppendAllText(_logFileLocation + $"\\IPAddressReporterLogs-{DateTimeOffset.Now:yyyy-MM-dd}.log", _logs.ToString());
		}
	}
}
