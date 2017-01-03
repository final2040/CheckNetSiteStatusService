using System;
using Data;

namespace Services
{
    public class NetworkMonitor2:INetworkMonitor
    {
        private readonly int _timeBetweenTests;
        private readonly int _timeOut;
        private readonly string _testNetworkName;
        private readonly INetworkTest _networkTest;

        private const string DEFAULT_NAME = "TestSite";
        private const int DEFAULT_TIME_BETWEEN_TESTS = 2000;
        private const int DEFAULT_TIMEOUT = 120000;

        public event EventHandler OnStatusChange;
        public event EventHandler OnConnectionLost;
        public event EventHandler OnConnectionBack;


        public INetworkTest NetworkTest
        {
            get { return _networkTest; }
        }

        public int TimeBetweenTests
        {
            get { return _timeBetweenTests; }
        }

        public int TimeOut
        {
            get { return _timeOut; }
        }

        public string TestNetworkName
        {
            get { return _testNetworkName; }
        }

        public NetworkMonitor2(string ip, INetworkTest networkTest) : this(new PingTest(ip), DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT,  DEFAULT_NAME) { }
        public NetworkMonitor2(string ip, int port, INetworkTest networkTest) : this(new TcpTest(ip, port), DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor2(string ip, int port, int timeBetweenTests, INetworkTest networkTest) : this(new TcpTest(ip, port), timeBetweenTests, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor2(string ip, int port, int timeBetweenTests, string name, INetworkTest networkTest) : this(new TcpTest(ip, port), timeBetweenTests, DEFAULT_TIMEOUT, name) { }
        public NetworkMonitor2(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds, string name, INetworkTest networkTest) : this(new TcpTest(ip, port), timeBetweenTests, timeUntilFailSeconds,  name) { }
        public NetworkMonitor2(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds, INetworkTest networkTest) : this(new TcpTest(ip, port), timeBetweenTests, timeUntilFailSeconds,  DEFAULT_NAME) { }
        public NetworkMonitor2(INetworkTest tester, INetworkTest networkTest) : this(tester, DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor2(INetworkTest tester, int timeBetweenTests, INetworkTest networkTest) : this(tester, timeBetweenTests, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor2(INetworkTest tester, TestConfig testConfiguration, string testName, INetworkTest networkTest) : this(tester, testConfiguration.WaitTimeSeconds * 1000, testConfiguration.TimeOutSeconds * 1000,testName) { }
        public NetworkMonitor2(INetworkTest tester, int timeBetweenTests, string name, INetworkTest networkTest) : this(tester, timeBetweenTests, DEFAULT_TIMEOUT, name) { }
        public NetworkMonitor2(INetworkTest tester, int timeBetweenTests, int timeOut, string testNetworkName)
        {
            _testNetworkName = testNetworkName;
            _networkTest = tester;
            _timeBetweenTests = timeBetweenTests;
            _timeOut = timeOut;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}