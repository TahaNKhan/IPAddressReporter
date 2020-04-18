using System;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.Logic.Services.GoDaddyDNSUpdateService.Models
{
	public class DNSRecordRequest
	{
		public string Data { get; set; }
		public int Port { get; set; }
		public int Priority { get; set; }
		public string Protocol { get; set; }
		public string Service { get; set; }
		public int TTL { get; set; }
		public int Weight { get; set; }
	}
}
