using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.Logic.Services.Interfaces
{
	public interface IServiceProxyFactory
	{
		IEmailService GetEmailService();
	}
}
