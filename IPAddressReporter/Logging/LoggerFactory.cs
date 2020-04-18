using IPAddressReporter.Configuration;

namespace IPAddressReporter.Logging
{
	public interface ILoggerFactory
	{
		ILogger BuildLogger();
	}
	public class LoggerFactory : ILoggerFactory
	{
		private readonly AppSettings _appSettings;
		public LoggerFactory(AppSettings appSettings)
		{
			_appSettings = appSettings;
		}

		public ILogger BuildLogger()
		{
			if (string.IsNullOrEmpty(_appSettings.LogFileLocation))
				return new ConsoleLogger();
			return new FileLogger(_appSettings);
		}
	}
}
