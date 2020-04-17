using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPAddressReporter.Logic.Services.Interfaces
{
	public interface IEmailService
	{
		Task SendEmailAsync(IEnumerable<string> recepients, string subject, string body, CancellationToken cancellationToken = default);
	}
}
