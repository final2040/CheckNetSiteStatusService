using System;
using System.Collections.Generic;

namespace Data
{
    public class TestNetworkEventArgs:EventArgs
    {
        public TestNetworkEventArgs(string connectionName, List<INetTestResult> netTestResults)
        {
            ConnectionName = connectionName;
            NetTestResults = netTestResults;
        }

        public string ConnectionName { get; private set; }
        public List<INetTestResult> NetTestResults { get; private set; }
    }
}