using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
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
        private Mock<PingTest> _networkTesterMock;

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

            _networkTesterMock = new Mock<PingTest>("172.28.129.100");
        }

        [Test]
        public void ShouldTestConnectionEvery30Milliseconds()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_successPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(150);

            // assert
            _networkTesterMock.Verify(nm => nm.Test(), Times.AtLeast(5));
            testThread.Abort();
        }

        [Test]
        public void ShouldChangeStatusToRetryingOnlineOnConnectionFail()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(120);

            // assert
            Assert.IsInstanceOf(typeof(RetryingOnline), monitor.CurrentState);
            testThread.Abort();
        }

        [Test]
        public void ShouldChangeStatusToOfflineAfter500MsUnsuccessfulRetries() // 20 retrys
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 500, "Test Network");
            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(750);

            // assert
            Assert.IsInstanceOf(typeof(Offline), monitor.CurrentState);
            _networkTesterMock.Verify(nm => nm.Test(), Times.AtLeast(16));
            testThread.Abort();
        }


        [Test]
        public void ShouldRaiseEventOnStatusChange()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_successPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 300, "Test Network");
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
            bool eventRaised = false;
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 10, 50, "Test Network");
            monitor.OnConnectionLost += (sender, args) => eventRaised = true;

            Thread testThread = new Thread(monitor.Start);

            // act
            testThread.Start();
            Thread.Sleep(100);

            // assert
            testThread.Abort();
            Assert.IsTrue(eventRaised);
            
        }

        [Test]
        public void ShouldRaiseEventReturnDataOfEventOnTimeout()
        {
            // arrange
            _networkTesterMock.Setup(nm => nm.Test()).Returns(_unsuccessPingResult).Verifiable();

            NetworkMonitor monitor = new NetworkMonitor(_networkTesterMock.Object, 30, 100, "Test Network");
            Thread testThread = new Thread(monitor.Start);
            TestNetworkEventArgs eventArgsResult = null;
            monitor.OnConnectionLost += (s, e) => { eventArgsResult = (TestNetworkEventArgs)e; };

            // act
            testThread.Start();
            Thread.Sleep(200);

            // assert
            Assert.AreEqual("Test Network", eventArgsResult.ConnectionName);
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
            Debug.Print("FirstAssertStatus = {0}", monitor.CurrentState);
            Assert.IsInstanceOf(typeof(Offline), monitor.CurrentState);

            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(30);
            Debug.Print("SecondAssertStatus = {0}", monitor.CurrentState);
            Assert.IsInstanceOf(typeof(RetryingOffline), monitor.CurrentState);

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
            Assert.IsInstanceOf(typeof(Online), monitor.CurrentState);
            _networkTesterMock.Verify(nm => nm.Test(), Times.AtLeast(16));
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
            Thread.Sleep(150);
            _unsuccessPingResult.Status = IPStatus.Success;
            Thread.Sleep(150);

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
