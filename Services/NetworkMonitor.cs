using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data;


namespace Services
{
    public class NetworkMonitor
    {
        #region Fields
        private ConnectionStatus _currentStatus = ConnectionStatus.ConnectionOnline;
        private readonly NetworkTest _networkTest;
        private readonly List<INetTestResult> _results = new List<INetTestResult>();
        private readonly int _timeBetweenTests;
        private readonly int _timeOut;
        private string _testNetworkName;
        private const string DEFAULT_NAME = "TestSite";
        private const int DEFAULT_TIME_BETWEEN_TESTS = 2000;
        private const int DEFAULT_TIMEOUT = 120000;
        #endregion

        #region Events
        public event EventHandler OnStatusChange;
        public event EventHandler OnConnectionLost;
        public event EventHandler OnConnectionBack;
        #endregion

        #region Properties
        public NetworkTest NetworkTest { get { return _networkTest; } }
        public int TimeBetweenTests { get { return _timeBetweenTests; } }
        public ConnectionStatus CurrentStatus
        {
            get { return _currentStatus; }
            private set
            {
                if (!value.Equals(_currentStatus))
                {
                    _currentStatus = value;
                    OnStatusChange?.Invoke(this, new ChangeEventArgs(_currentStatus,_testNetworkName));
                }
            }
        }
        #endregion

        #region Constructors

        public NetworkMonitor(string ip) : this(new NetworkTest(ip, -1), DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT,  DEFAULT_NAME) { }
        public NetworkMonitor(string ip, int port) : this(new NetworkTest(ip, port), DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests) : this(new NetworkTest(ip, port), timeBetweenTests, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, string name) : this(new NetworkTest(ip, port), timeBetweenTests, DEFAULT_TIMEOUT, name) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds, string name) : this(new NetworkTest(ip, port), timeBetweenTests, timeUntilFailSeconds,  name) { }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds) : this(new NetworkTest(ip, port), timeBetweenTests, timeUntilFailSeconds,  DEFAULT_NAME) { }
        public NetworkMonitor(NetworkTest tester) : this(tester, DEFAULT_TIME_BETWEEN_TESTS, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(NetworkTest tester, int timeBetweenTests) : this(tester, timeBetweenTests, DEFAULT_TIMEOUT, DEFAULT_NAME) { }
        public NetworkMonitor(NetworkTest tester, int timeBetweenTests, string name) : this(tester, timeBetweenTests, DEFAULT_TIMEOUT, name) { }
        public NetworkMonitor(NetworkTest tester, int timeBetweenTests, int timeOut, string testNetworkName)
        {
            _testNetworkName = testNetworkName;
            _networkTest = tester;
            _timeBetweenTests = timeBetweenTests;
            _timeOut = timeOut;
        }
        #endregion

        public void Start()
        {
            while (true)
            {
                TestConnection(CurrentStatus);
            }
        }

        private void TestConnection(ConnectionStatus status)
        {
            var startRetryingTime = DateTime.Now;

            var nextStatus = ConnectionStatus.ConnectionOffline;
            var testMode = false;

            if (status == ConnectionStatus.ConnectionOffline)
            {
                nextStatus = ConnectionStatus.ConnectionOnline;
                testMode = true;
            }

            while (CurrentStatus != nextStatus)
            {
                var testResult = NetworkTest.Test();
               
                if (testResult.Success == testMode && _currentStatus == status)
                {
                    CurrentStatus = ConnectionStatus.Retrying;
                    startRetryingTime = DateTime.Now;
                    _results.Clear();
                }
                if (CurrentStatus == ConnectionStatus.Retrying)
                {
                    _results.Add(testResult);
                    if (DateTime.Now.Subtract(startRetryingTime).TotalMilliseconds >= _timeOut &&
                        testResult.Success == testMode)
                    {
                        CurrentStatus = nextStatus;
                        RaiseEvent(status, _results);
                    }
                    else if (testResult.Success != testMode)
                    {
                        CurrentStatus = status;
                    }
                }

                Thread.Sleep(_timeBetweenTests);
            }
        }

        private void RaiseEvent(ConnectionStatus status, List<INetTestResult> retryResults)
        {
            if (status == ConnectionStatus.ConnectionOnline)
            {
                OnConnectionLost?.Invoke(this, new TestNetworkEventArgs(_testNetworkName, retryResults,NetworkTest.HostNameOrAddress,NetworkTest.RemotePort));
            }
            else
            {
                OnConnectionBack?.Invoke(this, new TestNetworkEventArgs(_testNetworkName, retryResults, NetworkTest.HostNameOrAddress, NetworkTest.RemotePort));
            }
        }
    }
}
