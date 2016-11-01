using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Data
{
    public class TestNetworkEventArgs:EventArgs
    {
        public TestNetworkEventArgs(string connectionName, List<INetTestResult> netTestResults, string hostNameOrAddress, string testConfig)
        {
            ConnectionName = connectionName;
            NetTestResults = netTestResults;
            HostNameOrAddress = hostNameOrAddress;
            TestConfig = testConfig;
        }

        public string HostNameOrAddress { get; set; }
        public string ConnectionName { get; private set; }
        public List<INetTestResult> NetTestResults { get; private set; }
        public string TestConfig { get; set; }
    }

   
}