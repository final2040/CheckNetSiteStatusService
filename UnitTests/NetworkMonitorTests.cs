using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Moq;
using NUnit.Framework;
using Services;

namespace UnitTests
{
    [TestFixture]
    public class NetworkMonitorTests
    {
        private PingTestResult _successPingResult;
        private PingTestResult _unsuccessPingResult;
        private Mock<NetworkTest> _networkTesterMock;

        [SetUp]
        public void Setup()
        {
            _successPingResult = new PingTestResult
            {
                Bytes = 32,
                Host = "172.28.129.100",
                Status = IPStatus.Success,
                Time = 2,
                Ttl = 128
            };
            _unsuccessPingResult = new PingTestResult
            {
                Bytes = 0,
                Host = "172.28.129.100",
                Status = IPStatus.TimedOut,
                Time = 0,
                Ttl = 128
            };

            _networkTesterMock = new Mock<NetworkTest>("172.28.129.100", 8080);
        }

        [Test]
        public void ShouldTestConnectionEveryHalfSecond()
        {
            // arrange
            _networkTesterMock.Setup(nm=> nm.Test()).Returns(_successPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30,"Test Network");
            Thread testThread = new Thread(monitor.Start);
            
            // act
            testThread.Start();
            Thread.Sleep(150);

            // assert
            _networkTesterMock.Verify(nm => nm.Test(),Times.AtLeast(5));
            testThread.Abort();
        }

        [Test]
        public void ShouldChangeStatusToRetryingIfConnectionReturnsFalse()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(120);

            // assert
            Assert.AreEqual(ConnectionStatus.Retrying, monitor.CurrentStatus);
            testThread.Abort();
        }

        [Test]
        public void ShouldChangeStatusToOfflineAfter500MsUnsuccessfulRetrys() // 20 retrys
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 500, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(750);

            // assert
            Assert.AreEqual(ConnectionStatus.ConnectionOffline, monitor.CurrentStatus);
            _networkTesterMock.Verify(nm => nm.Test(),Times.AtLeast(16));
            testThread.Abort();
        }

        [Test]
        public void ShouldChangeStatusToOnlineIfServiceBackOnlineBeforeTimeout()
        {
           // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 500, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(250);
            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(300);
            // assert
            Assert.AreEqual(ConnectionStatus.ConnectionOnline, monitor.CurrentStatus);
            testThread.Abort();
        }

        [Test]
        public void ShouldRaiseEventOnStatusChange()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_successPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 500, "Test Network");
            Thread testThread = new Thread(monitor.Start);
            bool eventRaised = false;
            monitor.OnStatusChange += (s, e) => { eventRaised = true; };
            
            // act
            testThread.Start();
            Thread.Sleep(250);
            _successPingResult.Status = IPStatus.TimedOut;
            Thread.Sleep(100);

            // assert
            Assert.IsTrue(eventRaised);
            testThread.Abort();
        }

        [Test]
        public void ShouldRaiseEventOnHostTimeout()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 500, "Test Network");
            Thread testThread = new Thread(monitor.Start);
            bool eventRaised = false;
            monitor.OnConnectionLost += (s, e) => { eventRaised = true; };

            // act
            testThread.Start();
            Thread.Sleep(750);

            // assert
            Assert.IsTrue(eventRaised);
            testThread.Abort();
        }

        [Test]
        public void ShouldRaiseEventReturnDataOfEventOnTimeout()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 150, "Test Network");
            Thread testThread = new Thread(monitor.Start);
            TestNetworkEventArgs eventArgsResult = null;
            monitor.OnConnectionLost += (s, e) => { eventArgsResult = (TestNetworkEventArgs) e; };

            // act
            testThread.Start();
            Thread.Sleep(200);

            // assert
            Assert.AreEqual("Test Network",eventArgsResult.ConnectionName);
            CollectionAssert.IsNotEmpty(eventArgsResult.NetTestResults);
            Assert.IsTrue(eventArgsResult.NetTestResults.Count() > 3);
            PrintEventArgsResult(eventArgsResult);

            testThread.Abort();
        }

        [Test]
        public void ShouldChangeFromOfflineToRetriyingStatus()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 150, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(200);
            Debug.Print("FirstAssertStatus = {0}",monitor.CurrentStatus);
            Assert.AreEqual(ConnectionStatus.ConnectionOffline,monitor.CurrentStatus);

            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(30);
            Debug.Print("SecondAssertStatus = {0}", monitor.CurrentStatus);
            Assert.AreEqual(ConnectionStatus.Retrying, monitor.CurrentStatus);

            testThread.Abort();
        }

        [Test]
        public void ShouldChangeStatusTOnlineAfter500MsSuccessfulRetrys() // 20 retrys
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 500, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(750);
            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(750);

            // assert
            Assert.AreEqual(ConnectionStatus.ConnectionOnline, monitor.CurrentStatus);
            _networkTesterMock.Verify(nm => nm.Test(), Times.AtLeast(16));
            testThread.Abort();
        }

        [Test]
        public void ShouldChangeStatusToOfflineIfServiceBackOfflineBeforeTimeout()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 500, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(750);
            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(300);
            _unsuccessPingResult.Status = IPStatus.TimedOut;
            Thread.Sleep(300);
            // assert
            Assert.AreEqual(ConnectionStatus.ConnectionOffline, monitor.CurrentStatus);
            testThread.Abort();
        }

        [Test]
        public void ShouldRaiseEventOnHostReconect()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 90, "Test Network");
            Thread testThread = new Thread(monitor.Start);
            bool eventRaised = false;
            monitor.OnConnectionBack += (s, e) => { eventRaised = true; };

            // act
            testThread.Start();
            Thread.Sleep(750);
            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(750);

            // assert
            Assert.IsTrue(eventRaised);
            testThread.Abort();
        }

        [Test]
        public void ShouldRaiseEventReturnDataOfEventOnConnect()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 150, "Test Network");
            Thread testThread = new Thread(monitor.Start);
            TestNetworkEventArgs eventArgsResult = null;
            monitor.OnConnectionBack += (s, e) => { eventArgsResult = (TestNetworkEventArgs)e; };

            // act
            testThread.Start();
            Thread.Sleep(200);
            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(200);

            // assert
            Assert.AreEqual("Test Network", eventArgsResult.ConnectionName);
            CollectionAssert.IsNotEmpty(eventArgsResult.NetTestResults);
            Assert.IsTrue(eventArgsResult.NetTestResults.Count() > 3);
            PrintEventArgsResult(eventArgsResult);

            testThread.Abort();
        }

        private void PrintEventArgsResult(TestNetworkEventArgs eventArgsResult)
        {
            if (eventArgsResult.NetTestResults != null)
            {
                Debug.Print(eventArgsResult.ConnectionName);
                foreach (var result in eventArgsResult.NetTestResults)
                {
                    Debug.Print(result.ToString());
                }
            }
        }
    }
}
