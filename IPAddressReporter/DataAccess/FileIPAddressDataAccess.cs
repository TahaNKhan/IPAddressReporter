using IPAddressReporter.DataAccess.Interfaces;
using IPAddressReporter.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.DataAccess
{
	public class FileIPAddressDataAccess : IIPAddressDataAccess
	{
		private const string FILE_NAME = "IPAddress.txt";

		/// <summary>
		/// Reads file IPAddress.txt from the same dir as the application and attempts
		/// to read the IP address from it.
		/// </summary>
		/// <returns>The IP Address read from the file, null for all other conditions</returns>
		public async Task<IPAddress> LoadIPAddress(ILogger logger = default)
		{
			if (!File.Exists(FILE_NAME))
			{
				logger?.LogInfo($"{FILE_NAME} does not exist");
				return null;
			}

			logger?.LogInfo($"Opening '{FILE_NAME}'");
			using var fileStream = File.OpenRead(FILE_NAME);
			using var fileReader = new StreamReader(fileStream);
			var fileStr = await fileReader.ReadToEndAsync();

			if (IPAddress.TryParse(fileStr, out var address))
			{
				logger?.LogInfo($"Found IP address in file '{FILE_NAME}'");
				return address;
			}

			logger?.LogInfo($"Unable to parse IPAddress, found text in '{FILE_NAME}', text: '{fileStr}'");
			return null;
		}

		/// <summary>
		/// Saves the IP address to the IPAddress.txt file.
		/// Creates the file if it doesn't exist.
		/// </summary>
		/// <param name="address">The IP Address to save</param>
		public async Task SaveIPAddress(IPAddress address, ILogger logger = default)
		{
			if (!File.Exists(FILE_NAME))
				logger?.LogInfo($"{FILE_NAME} does not exist, creating it.");
			
			// Overwrite existing file.
			using var fileStream = File.Create(FILE_NAME);
			using var streamWriter = new StreamWriter(fileStream);
			await streamWriter.WriteAsync($"{address}");
			logger?.LogInfo($"Wrote to file '{FILE_NAME}'");
		}
	}
}
