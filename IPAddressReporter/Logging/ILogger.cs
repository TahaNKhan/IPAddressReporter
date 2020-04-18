using System;

namespace IPAddressReporter.Logging
{
	public interface ILogger
	{
		void Log(string log);
		void Publish();

		static string FormatLog(string log)
		{
			log = log.Replace(Environment.NewLine, " ");
			log = log.Replace("\n", " ");
			log = log.Replace("\r", " ");
			return $"{DateTimeOffset.Now:MMM dd yyyy HH:mm:ss zzz} - {log}{Environment.NewLine}";
		}
	}
}
