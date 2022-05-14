using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IPAddressReporter.DataAccess
{
    public class IPReporterState
    {
        public string IPAddress { get; set; }
        public bool IsDNSUpdated { get; set; }
        public bool IsIPNotificationSent { get; set; }
    }
}
