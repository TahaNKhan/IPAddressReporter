using IPAddressReporter.Configuration;
using IPAddressReporter.Logging;
using IPAddressReporter.Logic;
using IPAddressReporter.Logic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter
{
	class Program
	{

		static async Task Main(string[] args)
		{
			var configurationFileName = GetConfigurationFileName();
			var config = new ConfigurationBuilder()
				.SetBasePath(Path.Combine(AppContext.BaseDirectory))
				.AddJsonFile(configurationFileName, optional: false)
				.Build();

			var appSettings = config.Get<AppSettings>();

			var serviceProvider = new ServiceCollection()
				.AddSingleton(appSettings)
				.AddSingleton<ILogger, FileLogger>()
				.AddSingleton<Logic.Services.Interfaces.IServiceProxyFactory, ServiceProxyFactory>()
				.AddSingleton<IReporterTask, ReporterTask>()
				.BuildServiceProvider();

			while (true)
			{
				var logger = serviceProvider.GetService<ILogger>();
				logger.Log("IP reporter started!");
				try
				{
					var mainTask = serviceProvider.GetService<IReporterTask>();
					var reportedIp = await mainTask.ReportIPAddress();
					if (reportedIp)
						logger.Log("IP change reported!");
					else
						logger.Log("IP address did not change");
					logger.Log("IP reporter finished successfully!");
				}
				catch (Exception ex)
				{
					logger.Log(ex.ToString());
				}
				finally
				{
					logger.Publish();
					Thread.Sleep(TimeSpan.FromSeconds(appSettings.WaitTimeSeconds));
				}
			}
		}

		static string GetConfigurationFileName()
		{
			const string releaseSettingsFileName = "appsettings.release.json";
			const string devSettingsFileName = "appsettings.json";
			if (CheckFileExistsInBuildDirectory(releaseSettingsFileName))
				return releaseSettingsFileName;
			return devSettingsFileName;
		}

		static bool CheckFileExistsInBuildDirectory(string fileName)
		{
			return File.Exists(AppContext.BaseDirectory + fileName);
		}
	}
}
