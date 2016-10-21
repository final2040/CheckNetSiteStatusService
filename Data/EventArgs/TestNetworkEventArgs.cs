using System;
using System.Collections.Generic;

namespace Data
{
    public class TestNetworkEventArgs:EventArgs
    {
        public TestNetworkEventArgs(string connectionName, List<INetTestResult> netTestResults, string ip, int port)
        {
            ConnectionName = connectionName;
            NetTestResults = netTestResults;
            Ip = ip;
            Port = port;
        }

        public string Ip { get; set; }
        public int Port { get; set; }
        public string ConnectionName { get; private set; }
        public List<INetTestResult> NetTestResults { get; private set; }
    }
}