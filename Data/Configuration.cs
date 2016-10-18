using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Configuration
    {
        public string LogFilePath { get; set; }
        public int WaitTime { get; set; }
        public int RetryTime { get; set; }
        public int ConnectionTimeout { get; set; }
        public IEnumerable<IP> Ips { get; set; }
        public MailConfig MailConfiguration { get; set; }
    }
}
