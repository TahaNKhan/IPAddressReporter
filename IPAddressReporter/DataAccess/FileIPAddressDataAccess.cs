using IPAddressReporter.DataAccess.Interfaces;
using IPAddressReporter.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.DataAccess
{
    public class FileIPAddressDataAccess : IIPAddressDataAccess
	{
        private string FILE_NAME
        {
            get
            {
                var directory = Directory.GetCurrentDirectory() + (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "/ip/" : Directory.GetCurrentDirectory() + "\\ip\\");
				Directory.CreateDirectory(directory);
				return directory + "IPAddress.txt";

			}
        }

        /// <summary>
        /// Reads file IPAddress.txt from the same dir as the application and attempts
        /// to read the IP address from it.
        /// </summary>
        /// <returns>The IP Address read from the file, null for all other conditions</returns>
        public async Task<IPReporterState> LoadIPReporterState(ILogger logger = default)
		{
			if (!File.Exists(FILE_NAME))
			{
				logger?.LogInfo($"{FILE_NAME} does not exist");
				return null;
			}

			try
			{
				logger?.LogInfo($"Opening '{FILE_NAME}'");
				using var fileStream = File.OpenRead(FILE_NAME);
				using var fileReader = new StreamReader(fileStream);
				var fileStr = await fileReader.ReadToEndAsync();
				var deserialized = JsonSerializer.Deserialize<IPReporterState>(fileStr);
				return deserialized;
			}
			catch (Exception)
			{
				logger?.LogInfo($"Unable to parse Reporter State, found text in '{FILE_NAME}'");
				return null;
			}
		}

		/// <summary>
		/// Saves the IP address to the IPAddress.txt file.
		/// Creates the file if it doesn't exist.
		/// </summary>
		/// <param name="ipReporterState">The IP Address to save</param>
		public async Task SaveIPAddress(IPReporterState ipReporterState, ILogger logger = default)
		{
			if (!File.Exists(FILE_NAME))
				logger?.LogInfo($"{FILE_NAME} does not exist, creating it.");
			
			// Overwrite existing file.
			using var fileStream = File.Create(FILE_NAME);
			using var streamWriter = new StreamWriter(fileStream);
			await streamWriter.WriteAsync(JsonSerializer.Serialize(ipReporterState));
			logger?.LogInfo($"Wrote to file '{FILE_NAME}'");
		}
	}
}
