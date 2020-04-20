using IPAddressReporter.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.DataAccess
{
	public class FileDataContext: IDataContext
	{
		public IIPAddressDataAccess GetIPAddressDataAccess() => new FileIPAddressDataAccess();

		/// <summary>
		/// For when a real db is being used.
		/// </summary>
		public void Dispose() { }
	}
}
