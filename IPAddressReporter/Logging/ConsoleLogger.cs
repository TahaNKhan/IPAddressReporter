using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.Logging
{
	public class ConsoleLogger : ILogger
	{
		void ILogger.Log(string log) => Console.WriteLine(log);

		public void Publish() { }
	}
}
