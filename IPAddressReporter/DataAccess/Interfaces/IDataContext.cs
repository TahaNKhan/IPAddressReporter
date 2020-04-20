using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.DataAccess.Interfaces
{
	public interface IDataContext: IDisposable
	{
		IIPAddressDataAccess GetIPAddressDataAccess();
	}
}
