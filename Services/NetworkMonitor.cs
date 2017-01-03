using System;
using System.Collections.Generic;
using System.Threading;
using Data;

namespace Services
{
    public class NetworkMonitor : NetworkMonitorBase
    {
        #region Fields

        #endregion

        #region Events
        public override event EventHandler OnStatusChange;
        public override event EventHandler OnConnectionLost;
        public override event EventHandler OnConnectionBack;
        #endregion

        #region Properties

        #endregion

        #region Constructors

        public NetworkMonitor(string ip) : base(ip) {
        }
        public NetworkMonitor(string ip, int port) : base(ip, port) {
        }
        public NetworkMonitor(string ip, int port, int timeBetweenTests) : base(ip, port, timeBetweenTests) {
        }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, string name) : base(ip, port, timeBetweenTests, name) {
        }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds, string name) : base(ip, port, timeBetweenTests, timeUntilFailSeconds, name) {
        }
        public NetworkMonitor(string ip, int port, int timeBetweenTests, int timeUntilFailSeconds) : base(ip, port, timeBetweenTests, timeUntilFailSeconds) {
        }
        public NetworkMonitor(INetworkTest tester) : base(tester) {
        }
        public NetworkMonitor(INetworkTest tester, int timeBetweenTests) : base(tester, timeBetweenTests) {
        }
        public NetworkMonitor(INetworkTest tester, TestConfig testConfiguration, string testName) : base(tester, testConfiguration, testName) {
        }
        public NetworkMonitor(INetworkTest tester, int timeBetweenTests, string name) : base(tester, timeBetweenTests, name) {
        }
        public NetworkMonitor(INetworkTest tester, int timeBetweenTests, int timeOut, string testNetworkName) : base(tester, timeBetweenTests, timeOut, testNetworkName)
        {
        }
        #endregion

        public override void Start()
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
                    _currentStatus = ConnectionStatus.Retrying;
                    startRetryingTime = DateTime.Now;
                    _results.Clear();
                }
                if (CurrentStatus == ConnectionStatus.Retrying)
                {
                    _results.Add(testResult);
                    if (DateTime.Now.Subtract(startRetryingTime).TotalMilliseconds >= _timeOut &&
                        testResult.Success == testMode)
                    {
                        _currentStatus = nextStatus;
                        RaiseEvent(status, _results);
                    }
                    else if (testResult.Success != testMode)
                    {
                        _currentStatus = status;
                    }
                }

                Thread.Sleep(_timeBetweenTests);
            }
        }

        private void RaiseEvent(ConnectionStatus status, List<INetTestResult> retryResults)
        {
            if (status == ConnectionStatus.ConnectionOnline)
            {
                OnConnectionLost?.Invoke(this, new TestNetworkEventArgs(_testNetworkName, retryResults,NetworkTest.HostNameOrAddress, NetworkTest.TestConfiguration.ToString()));
            }
            else
            {
                OnConnectionBack?.Invoke(this, new TestNetworkEventArgs(_testNetworkName, retryResults,NetworkTest.HostNameOrAddress, NetworkTest.TestConfiguration.ToString()));
            }
        }
    }
}
