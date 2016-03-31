using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AureliaAspNetCoreAuth.Models
{
    public class AppSettings
    {
        public UriSettings UriSettings { get; set; }
        public Logging Logging { get; set; }
    }

    public class UriSettings
    {
        public string ClientUri { get; set; }
        public string ServerURi { get; set; }
    }

    public class Logging
    {
        public bool IncludeScopes { get; set; }
        public LogLevel LogLevel { get; set; }
    }


    public class LogLevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }
}