using System.Collections.Generic;

namespace Data.NetworkTest
{
    public class TestNetworkEventArgs:System.EventArgs
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