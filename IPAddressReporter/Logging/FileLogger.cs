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

		void ILogger.Log(string str)
		{
			_logs.Append(str);
		}

		public void Publish()
		{
			var logFileName = _logFileLocation + $"\\IPAddressReporterLogs-{DateTimeOffset.Now:yyyy-MM-dd}.log";
			File.AppendAllText(logFileName, _logs.ToString());
			_logs.Clear();
		}
	}
}
