using System;
using System.Collections.Generic;
using Data;

namespace Services
{
    public abstract class NetworkMonitorBase
    {
        private const string DEFAULT_NAME = "TestSite";
        private const int DEFAULT_TIME_BETWEEN_TESTS = 2000;
        private const int DEFAULT_TIMEOUT = 120000;
        protected ConnectionStatus _currentStatus = ConnectionStatus.ConnectionOnline;
        protected readonly List<INetTestResult> _results = new List<INetTestResult>();
        protected int _timeBetweenTests;
        protected int _timeOut;
        protected string _testNetworkName;


        public NetworkMonitorBase(string ip) : this(new PingTest(ip), DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT,  DEFAULT_NAME) { }
        public NetworkMonitorBase(string ip, int port) : this((INetworkTest) new TcpTest(ip, port), (int) DEFAULT_TIME_BETWEEN_TESTS, (int) DEFAULT_TIMEOUT, (string) DEFAULT_NAME) { }
        public NetworkMonitorBase(string ip, int port, int timeBetweenTests) : this((INetworkTest) new TcpTest(ip, port), timeBetweenTests, (int) DEFAULT_TIMEOUT, (string) DEFAULT_NAME) { }
        public NetworkMonitorBase(string ip, int port, int timeBetweenTests, string name) : this(new TcpTest(ip, port), timeBetweenTests, DEFAULT_TIMEOUT, name) { }
        public NetworkMonitorBase(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds, string name) : this(new TcpTest(ip, port), timeBetweenTests, timeUntilFailSeconds,  name) { }
        public NetworkMonitorBase(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds) : this((INetworkTest) new TcpTest(ip, port), timeBetweenTests, timeUntilFailSeconds,  (string) DEFAULT_NAME) { }
        public NetworkMonitorBase(INetworkTest tester) : this(tester, (int) DEFAULT_TIME_BETWEEN_TESTS, (int) DEFAULT_TIMEOUT, (string) DEFAULT_NAME) { }
        public NetworkMonitorBase(INetworkTest tester, int timeBetweenTests) : this(tester, timeBetweenTests, (int) DEFAULT_TIMEOUT, (string) DEFAULT_NAME) { }
        public NetworkMonitorBase(INetworkTest tester, TestConfig testConfiguration, string testName) : this(tester, testConfiguration.WaitTimeSeconds * 1000, testConfiguration.TimeOutSeconds * 1000,testName) { }
        public NetworkMonitorBase(INetworkTest tester, int timeBetweenTests, string name) : this(tester, timeBetweenTests, (int) DEFAULT_TIMEOUT, name) { }

        public NetworkMonitorBase(INetworkTest tester, int timeBetweenTests, int timeOut, string testNetworkName)
        {
            _testNetworkName = testNetworkName;
            NetworkTest = tester;
            _timeBetweenTests = timeBetweenTests;
            _timeOut = timeOut;
        }

        public virtual event EventHandler OnStatusChange;
        public virtual event EventHandler OnConnectionLost;
        public virtual event EventHandler OnConnectionBack;

        public INetworkTest NetworkTest { get; set; }
        public int TimeBetweenTests => _timeBetweenTests;

        

        public abstract void Start();
    }
}