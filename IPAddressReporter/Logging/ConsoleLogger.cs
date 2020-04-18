using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.Logging
{
	public class ConsoleLogger : ILogger
	{
		public void Log(string log) => Console.WriteLine(ILogger.FormatLog(log));

		public void Publish() { }
	}
}
