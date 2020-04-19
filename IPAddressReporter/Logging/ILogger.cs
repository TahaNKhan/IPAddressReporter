using System;

namespace IPAddressReporter.Logging
{
	public interface ILogger
	{
		protected void Log(string log);
		void LogInfo(string log) => Log(FormatLog($"[Info] {log}"));
		void LogError(string errorInfo) => Log(FormatLog($"[Error] {errorInfo}"));
		void Publish();
		string FormatLog(string log)
		{
			log = log.Replace(Environment.NewLine, " ");
			log = log.Replace("\n", " ");
			log = log.Replace("\r", " ");
			return $"{DateTimeOffset.Now:MMM dd yyyy HH:mm:ss zzz} - {log}{Environment.NewLine}";
		}
	}
}
