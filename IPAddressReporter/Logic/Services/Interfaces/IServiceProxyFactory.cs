namespace IPAddressReporter.Logic.Services.Interfaces
{
	public interface IServiceProxyFactory
	{
		IEmailService GetEmailService();
		IDNSUpdateService GetDNSUpdateService();
	}
}
