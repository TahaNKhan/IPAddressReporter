using IPAddressReporter.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.DataAccess
{
	public class FileDataContextFactory : IDataContextFactory
	{
		public IDataContext Construct() => new FileDataContext();
	}
}
