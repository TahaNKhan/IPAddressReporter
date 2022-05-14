using IPAddressReporter.Configuration;
using System;
using System.IO;
using System.Runtime.InteropServices;
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
			EnsureDirectoryExists(_logFileLocation);
			_logs = new StringBuilder();
		}

		void ILogger.Log(string str)
		{
			_logs.Append(str);
		}

		public void Publish()
		{
			var logFileName = GetLogFileName();
			File.AppendAllText(logFileName, _logs.ToString());
			_logs.Clear();
		}

        private void EnsureDirectoryExists(string logFileLocation)
        {
			Directory.CreateDirectory(logFileLocation);
		}

        private string GetLogFileName()
		{
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				return _logFileLocation + $"/IPAddressReporterLogs-{DateTimeOffset.Now:yyyy-MM-dd}.log";

			return _logFileLocation + $"\\IPAddressReporterLogs-{DateTimeOffset.Now:yyyy-MM-dd}.log";
		}
	}
}
