using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.Logging
{
	public interface ILogger
	{
		void Log(string log);
		void Publish();
	}
}
